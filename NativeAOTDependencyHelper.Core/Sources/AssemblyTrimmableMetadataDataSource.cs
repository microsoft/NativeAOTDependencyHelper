using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using Nito.Disposables.Internals;
using NuGet.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NativeAOTDependencyHelper.Core.Sources
{
    public class AssemblyTrimmableMetadataDataSource(TaskOrchestrator _taskOrchestrator, ILogger _logger) : IDataSource<bool>
    {
        public string Name => "If package assembly metadata is marked as trimmable";

        public string Description => "Returns the custom metadata attributes indicated on the NuGet package's assembly";

        // No need to setup for network call-- returns true by default
        public bool IsInitialized => true;

        public async Task<bool> InitializeAsync() => true;

        public async Task<bool> GetInfoForPackageAsync(NuGetPackageInfo package, CancellationToken cancellationToken)
        {
            var globalPackagePath = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(root: null));
            var packagePath = Path.Combine(globalPackagePath, package.Name.ToLower(), package.ResolvedVersion);
            AppDomain currentDomain = AppDomain.CurrentDomain;

            if (Directory.Exists(packagePath))
            {
                string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
                string[] paths = Directory.GetFiles(packagePath, "*.dll", SearchOption.AllDirectories);
                var resolver = new PathAssemblyResolver(runtimeAssemblies.Concat(paths));
                using var context = new MetadataLoadContext(resolver);

                foreach (var path in paths)
                {
                    try
                    {
                        Assembly[]? assemblies = currentDomain.GetAssemblies()
                            .Concat(context.GetAssemblies())
                            .Where(a => a.GetName().Name == package.Name)
                            .ToArray();
                        var assembly = (assemblies == null || assemblies.Length == 0) ? context.LoadFromAssemblyPath(path) : assemblies.First();
                        var attributes = assembly.GetCustomAttributesData().Where(a => a.AttributeType.Name == typeof(AssemblyMetadataAttribute).Name);

                        foreach (var attribute in attributes)
                        {
                            if (attribute.ConstructorArguments.Count == 2 &&
                                attribute.ConstructorArguments[0].Value?.ToString()?.ToLower() == "istrimmable")
                            {
                                return attribute.ConstructorArguments[1].Value?.ToString()?.ToLower() == "true";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, $"Error loading assembly {path}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Package {package.Name} {package.ResolvedVersion} not found in the global packages folder.");
            }
            return false;
        }
    }
}

using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NativeAOTDependencyHelper.Core.Sources
{
    public class AssemblyMetadataDatasource(TaskOrchestrator _taskOrchestrator, ILogger _logger) : IDataSource<List<AssemblyMetadataAttribute>>
    {
        public string Name => "Package Assembly Metadata";

        public string Description => "Returns the custom metadata attributes indicated on the NuGet package's assembly";

        // No need to setup for network call-- returns true by default
        public bool IsInitialized => true;

        public bool GetAssemblyMetadata(string packageName, string version)
        {
            var globalPackagePath = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(root: null));
            var packagePath = Path.Combine(globalPackagePath, packageName.ToLower(), version);


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
                        Assembly assembly = context.LoadFromAssemblyPath(path);
                        var attributes = assembly.GetCustomAttributesData();

                        foreach (var attribute in attributes)
                        {
                            if (attribute.AttributeType.Name == typeof(AssemblyMetadataAttribute).Name &&
                                attribute.ConstructorArguments.Count == 2 &&
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
                Console.WriteLine($"Package {packageName} {version} not found in the global packages folder.");
            }
            return false;
        }

        public async Task<List<AssemblyMetadataAttribute>?> GetInfoForPackageAsync(NuGetPackageInfo package, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InitializeAsync() => true;
    }
}

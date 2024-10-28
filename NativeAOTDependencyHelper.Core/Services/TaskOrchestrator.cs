using NativeAOTDependencyHelper.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace NativeAOTDependencyHelper.Core.Services;

/// <summary>
/// Helper class which orchestrates/reports on all tasks and caches results for reports/checks.
/// </summary>
public class TaskOrchestrator(SolutionPackageIndex _servicePackageIndex, IServiceProvider _serviceProvider)
{
    public event EventHandler<ProcessingPackageEventArgs>? StartedProcessingPackage;

    public event EventHandler<ProcessingPackageEventArgs>? FinishedProcessingPackage;
    
    public event EventHandler<ReportPackageProgressEventArgs>? ReportPackageProgress;

    public int NumberOfProviders { get; private set; }

    public async Task<bool> ProcessSolutionAsync(string solutionFilePath)
    {
        if (await _servicePackageIndex.InitializeAsync(solutionFilePath)
            && _servicePackageIndex.Packages != null)
        {
            var providers = _serviceProvider.GetServices<IReportItemProvider>().ToArray();

            NumberOfProviders = providers.Length;

            List<Task> tasks = new();

            foreach (var package in _servicePackageIndex.Packages)
            {
                StartedProcessingPackage?.Invoke(this, new(package));

                // https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
                // https://sergeyteplyakov.github.io/Blog/async/2019/05/21/The-Dangers-of-Task.Factory.StartNew.html
                // https://learn.microsoft.com/archive/msdn-magazine/2011/february/msdn-magazine-parallel-computing-it-s-all-about-the-synchronizationcontext
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    foreach (var reporter in providers)
                    {
                        // TODO: Do we want a background task per reporter to be tracking (and do this in parallel)?
                        var report = await reporter.ProcessPackage(package);

                        ReportPackageProgress?.Invoke(this, new(package, report));
                    }

                    // TODO: If we do background the reporters, then we'll want to wait for them all to be done before reporting the package is finished...
                    FinishedProcessingPackage?.Invoke(this, new(package));
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // TODO: Do we want to record time to complete?

            return true;
        }

        return false;
    }

    public async Task<T?> GetDataFromSourceForPackageAsync<T>(IDataSource<T> dataSource, NuGetPackageInfo package)
    {
        // TODO: We should lock on datasource here, as we only need to initialize once.
        // TODO: We may want to investigate if we can grab all the generic reporter interfaces from the services collection and intialize them before we start processing instead...
        if (!dataSource.IsInitialized)
        {
            await dataSource.InitializeAsync();
        }

        // TODO: We should cache based on the package and only call this once per package...
        return await dataSource.GetInfoForPackageAsync<T>(package);
    }
}

public class ProcessingPackageEventArgs(NuGetPackageInfo _nugetPackage) : EventArgs
{
    public NuGetPackageInfo Package => _nugetPackage;
}

public class ReportPackageProgressEventArgs(NuGetPackageInfo _nugetPackage, ReportItem _reportItem) : EventArgs
{
    public NuGetPackageInfo Package = _nugetPackage;

    public ReportItem ReportItem => _reportItem;
}
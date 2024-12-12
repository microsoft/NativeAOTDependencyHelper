﻿using NativeAOTDependencyHelper.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using System.Collections.Concurrent;
using Microsoft.Build.Locator;

namespace NativeAOTDependencyHelper.Core.Services;

/// <summary>
/// Helper class which orchestrates/reports on all tasks and caches results for reports/checks.
/// </summary>
public class TaskOrchestrator(SolutionPackageIndex _servicePackageIndex, IServiceProvider _serviceProvider, ILogger _logger)
{
    public event EventHandler<ProcessingPackageEventArgs>? StartedProcessingPackage;

    public event EventHandler<ProcessingPackageEventArgs>? FinishedProcessingPackage;
    
    public event EventHandler<ReportPackageProgressEventArgs>? ReportPackageProgress;

    public int NumberOfProviders { get; private set; }

    private ConcurrentDictionary<Type, AsyncLock> _dataSourceInitializeLocks = new();

    private ConcurrentDictionary<(Type, NuGetPackageInfo), object?> _resultCache = new();

    public async Task<bool> ProcessSolutionAsync(string solutionFilePath)
    {
        _logger.Information($"Initializing Solution: {solutionFilePath}");

        if (!MSBuildLocator.IsRegistered)
        {
            var instance = MSBuildLocator.RegisterDefaults();
            Console.WriteLine($"Registered MSBuild from: {instance.MSBuildPath}");
        }

        if (await _servicePackageIndex.InitializeAsync(solutionFilePath)
            && _servicePackageIndex.Packages != null
            && _servicePackageIndex.Packages.Any())
        {
            var providers = _serviceProvider.GetServices<IReportItemProvider>().ToArray();

            NumberOfProviders = providers.Length;

            List<Task> tasks = new();

            foreach (var package in _servicePackageIndex.Packages)
            {
                _logger.Information($"Processing Package: {package.Name}");
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
                    _logger.Information($"Finished Package: {package.Name}");
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // TODO: Do we want to record time to complete?

            return true;
        }

        _logger.Warning("No Packages to Process or Issue Loading Package Index");

        return false;
    }

    public async Task<T?> GetDataFromSourceForPackageAsync<T>(IDataSource<T> dataSource, NuGetPackageInfo package)
    {
        // TODO: We may want to investigate if we can grab all the generic reporter interfaces from the services collection and initialize them before we start processing instead...
        using (await _dataSourceInitializeLocks.GetOrAdd(typeof(T), new AsyncLock()).LockAsync())
        {
            if (!dataSource.IsInitialized)
            {
                _logger.Information($"Initializing DataSource: {dataSource.Name}");
                await dataSource.InitializeAsync();
            }

            // TODO: We don't need to lock the whole datasource for this... but then it seems excessive to lock on every pairing here... (even though that's what we need). Think about the approach here more.
            // Note: We CANNOT use GetOrAdd on our ConcurrentDictionary here as that doesn't guarantee that the factory method will only be called once.
            if (_resultCache.TryGetValue((dataSource.GetType(), package), out var result))
            {
                _logger.Information($"Returning Cache: {dataSource.Name} - {package.Name}");
                return (T?)result;
            }
            else
            {
                _logger.Information($"Fetching Data: {dataSource.Name} - {package.Name}");
                var resultNew = await dataSource.GetInfoForPackageAsync(package);
                _resultCache[(dataSource.GetType(), package)] = resultNew;
                return resultNew;
            }
        }
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
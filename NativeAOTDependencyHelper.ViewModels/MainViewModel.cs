using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System.Collections.ObjectModel;

namespace NativeAOTDependencyHelper.ViewModels;

public partial class MainViewModel(IServiceProvider _serviceProvider, TaskScheduler _uiScheduler, DotnetToolingInterop _dotnetInterop, ILogger _logger) : ObservableObject
{
    public IAsyncRelayCommand DotnetVersionCommand { get; } = new AsyncRelayCommand(_dotnetInterop.CheckDotnetVersion);

    [ObservableProperty]
    public partial ObservableCollection<NuGetPackageViewModel> Packages { get; set; } = new();

    [ObservableProperty]
    public partial int PackagesProcessed { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalChecks))]
    [NotifyPropertyChangedFor(nameof(IsViewEmpty))]
    public partial int TotalPackages { get; set; }

    public int ChecksPerPackage => _taskOrchestrator?.NumberOfProviders ?? 0;

    [ObservableProperty]
    public partial int ChecksProcessed { get; set; }

    public int TotalChecks => TotalPackages * ChecksPerPackage;

    [ObservableProperty]
    public partial bool IsWorking { get; set; }

    public bool IsViewEmpty => Packages.Count == 0;

    private TaskOrchestrator? _taskOrchestrator;

    public IReportItemProvider[]? GetReportAndCheckTypes => _serviceProvider?.GetServices<IReportItemProvider>().ToArray();

    // TODO: Have error string to report back issues initializing?

    [RelayCommand]
    public async Task<bool> ProcessSolutionFileAsync(string filepath)
    {
        _taskOrchestrator = _serviceProvider?.GetService<TaskOrchestrator>();

        // Ensure we start fresh each time!
        Packages.Clear();
        PackagesProcessed = 0;
        TotalPackages = 0;
        ChecksProcessed = 0;        

        if (_taskOrchestrator != null)
        {
            // Keep track of what's happening, progress, and completion
            _taskOrchestrator.StartedProcessingPackage += _taskOrchestrator_StartedProcessingPackage;
            _taskOrchestrator.ReportPackageProgress += _taskOrchestrator_ReportPackageProgress;
            _taskOrchestrator.FinishedProcessingPackage += _taskOrchestrator_FinishedProcessingPackage;

            IsWorking = true;

            var result = await _taskOrchestrator.ProcessSolutionAsync(filepath);

            if (!result)
            {
                _logger.Error(new InvalidOperationException($"There was an issue processing the solution {filepath}"), "Processing Error");
            }

            return result;
        }

        return false;
    }

    private void _taskOrchestrator_StartedProcessingPackage(object? sender, ProcessingPackageEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            // Add our package to our list now that it's started processing.
            Packages.Add(new(e.Package, ChecksPerPackage));
            // Keep track of how many packages we have.
            TotalPackages++;
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler).Wait();
    }

    private void _taskOrchestrator_ReportPackageProgress(object? sender, ReportPackageProgressEventArgs e)
    {
        // Need to report/modify the collection on the UI thread.
        // https://learn.microsoft.com/archive/msdn-magazine/2011/february/msdn-magazine-parallel-computing-it-s-all-about-the-synchronizationcontext
        Task reportProgressTask = Task.Factory.StartNew(() =>
        {
            // TODO: Make a better lookup here or can we just have an ObservableDictionary?
            var package = Packages.FirstOrDefault(p => p.Info == e.Package);

            if (package != null)
            {
                if (e.ReportItem is AOTCheckItem check)
                {
                    package.CheckItems.Add(check);
                }
                else if (e.ReportItem is ReportItem item)
                {
                    package.ReportItems.Add(item);
                }

                if (e.ReportItem.ProcessingError != null)
                {
                    package.ProcessingErrors.Add(e.ReportItem.ProcessingError);
                    package.LoadStatus = PackageLoadStatus.Error;
                }
                package.ReportsCompleted++;
            }

            ChecksProcessed++;
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);

        reportProgressTask.Wait();
    }

    private void _taskOrchestrator_FinishedProcessingPackage(object? sender, ProcessingPackageEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            
            // Keep track of how many packages we've processed
            PackagesProcessed++;
            var package = Packages.FirstOrDefault(p => p.Info == e.Package);
            if (package != null)
            {
                if (package.ProcessingErrors.Count > 0)
                {
                    package.LoadStatus = PackageLoadStatus.Error;
                }
                else
                {
                    package.LoadStatus = PackageLoadStatus.Success;
                }
            }

            // All of our processing is done!
            if (PackagesProcessed == TotalPackages)
            {
                IsWorking = false;
                if (_taskOrchestrator != null)
                {
                    _taskOrchestrator.StartedProcessingPackage -= _taskOrchestrator_StartedProcessingPackage;
                    _taskOrchestrator.ReportPackageProgress -= _taskOrchestrator_ReportPackageProgress;
                    _taskOrchestrator.FinishedProcessingPackage -= _taskOrchestrator_FinishedProcessingPackage;
                }
            }
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler).Wait();
    }    
}

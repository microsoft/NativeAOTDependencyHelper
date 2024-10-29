using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NativeAOTDependencyHelper.ViewModels;

public partial class MainViewModel(IServiceProvider _serviceProvider, TaskScheduler _uiScheduler) : ObservableObject
{
    public IAsyncRelayCommand DotnetVersionCommand { get; } = new AsyncRelayCommand(DotnetToolingInterop.CheckDotnetVersion);

    [ObservableProperty]
    private ObservableCollection<NuGetPackageViewModel> _packages = new();

    [ObservableProperty]
    private int _packagesProcessed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalChecks))]
    private int _totalPackages;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalChecks))]
    private int _checksPerPackage;

    [ObservableProperty]
    private int _checksProcessed;

    public int TotalChecks => TotalPackages * ChecksPerPackage;

    [ObservableProperty]
    private bool _isWorking;

    private TaskOrchestrator? _taskOrchestrator;

    // TODO: Have error string to report back issues initializing?

    [RelayCommand]
    public async Task<bool> ProcessSolutionFileAsync(string filepath)
    {
        // Ensure we start fresh each time!
        Packages.Clear();
        PackagesProcessed = 0;
        TotalPackages = 0;
        ChecksProcessed = 0;

        _taskOrchestrator = _serviceProvider?.GetService<TaskOrchestrator>();

        if (_taskOrchestrator != null)
        {
            // TODO: Need to know when all the packages are finished and remove these...
            _taskOrchestrator.StartedProcessingPackage += _taskOrchestrator_StartedProcessingPackage;
            _taskOrchestrator.ReportPackageProgress += _taskOrchestrator_ReportPackageProgress;
            _taskOrchestrator.FinishedProcessingPackage += _taskOrchestrator_FinishedProcessingPackage;

            IsWorking = true;

            await _taskOrchestrator.ProcessSolutionAsync(filepath);

            ChecksPerPackage = _taskOrchestrator.NumberOfProviders;

            return true;
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

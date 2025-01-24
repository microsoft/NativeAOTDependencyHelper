// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System.Collections.ObjectModel;

namespace NativeAOTDependencyHelper.ViewModels;

public partial class MainViewModel(IServiceProvider _serviceProvider, TaskScheduler _uiScheduler, DotnetToolingInterop _dotnetInterop, ILogger _logger, CredentialManager _credentialManager) : ObservableObject
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
    [NotifyPropertyChangedFor(nameof(IsOpenSolutionEnabled))]
    public partial bool IsWorking { get; set; }

    public bool IsViewEmpty => Packages.Count == 0;

    public bool IsOpenSolutionEnabled => (!IsWorking || IsCancelled) && _credentialManager.HasCredential && (IsTaskSuccessful(DotnetVersionCommand.ExecutionTask.Status));

    private TaskOrchestrator? _taskOrchestrator;

    public IReportItemProvider[]? GetReportAndCheckTypes => _serviceProvider?.GetServices<IReportItemProvider>().ToArray();

    private static bool IsTaskSuccessful(TaskStatus status) => status == TaskStatus.RanToCompletion;

    private CancellationTokenSource? _cancellationToken;

    public bool IsCancelled => _cancellationToken?.IsCancellationRequested ?? false;

    public void CancelProcess()
    {
        if (_cancellationToken != null)
        {
            foreach (NuGetPackageViewModel package in Packages)
            {
                if (package.LoadStatus == PackageLoadStatus.Loading)
                {
                    package.LoadStatus = PackageLoadStatus.Cancelled;
                }
            }

            OnProcessFinished();
            UpdateIsOpenSolutionEnabledProperty();
        }
    }

    public void OnProcessFinished()
    {
        IsWorking = false;
        if (_taskOrchestrator != null)
        {
            _taskOrchestrator.StartedProcessingPackage -= _taskOrchestrator_StartedProcessingPackage;
            _taskOrchestrator.ReportPackageProgress -= _taskOrchestrator_ReportPackageProgress;
            _taskOrchestrator.FinishedProcessingPackage -= _taskOrchestrator_FinishedProcessingPackage;
        }
        _cancellationToken?.Cancel();
        _cancellationToken?.Dispose();
        _cancellationToken = null;
    }

 
    // TODO: Have error string to report back issues initializing?

    public void UpdateIsOpenSolutionEnabledProperty() => OnPropertyChanged(nameof(IsOpenSolutionEnabled));

    [RelayCommand]
    public async Task<bool> ProcessSolutionFileAsync(string filepath)
    {
        _taskOrchestrator = _serviceProvider?.GetService<TaskOrchestrator>();

        // Ensure we start fresh each time!
        Packages.Clear();
        PackagesProcessed = 0;
        TotalPackages = 0;
        ChecksProcessed = 0;

        _cancellationToken = new CancellationTokenSource();

        if (_taskOrchestrator != null)
        {
            // Keep track of what's happening, progress, and completion
            _taskOrchestrator.StartedProcessingPackage += _taskOrchestrator_StartedProcessingPackage;
            _taskOrchestrator.ReportPackageProgress += _taskOrchestrator_ReportPackageProgress;
            _taskOrchestrator.FinishedProcessingPackage += _taskOrchestrator_FinishedProcessingPackage;

            IsWorking = true;

            var result = await _taskOrchestrator.ProcessSolutionAsync(filepath, _cancellationToken.Token);

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
        }, _cancellationToken == null ? CancellationToken.None : _cancellationToken.Token, TaskCreationOptions.None, _uiScheduler).Wait();
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

                if (e.ReportItem.ProcessingError != null
                    || (e.ReportItem is AOTCheckItem check2 && check2.Status == CheckStatus.Error))
                {
                    package.ProcessingErrors.Add(e.ReportItem.ProcessingError ?? e.ReportItem.ReportDetails);
                    package.LoadStatus = PackageLoadStatus.Error;
                }
                package.ReportsCompleted++;
            }

            ChecksProcessed++;
        }, _cancellationToken == null ? CancellationToken.None : _cancellationToken.Token, TaskCreationOptions.None, _uiScheduler);

        reportProgressTask.Wait();
    }

    private void _taskOrchestrator_FinishedProcessingPackage(object? sender, ProcessingPackageEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {

            // Keep track of how many packages we've processed
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
                PackagesProcessed++;
            }

            // All of our processing is done!
            if (PackagesProcessed == TotalPackages)
            {
                OnProcessFinished();
            }
        }, _cancellationToken == null ? CancellationToken.None : _cancellationToken.Token, TaskCreationOptions.None, _uiScheduler).Wait();

    }
}

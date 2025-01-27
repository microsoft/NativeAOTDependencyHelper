// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using NativeAOTDependencyHelper.Core.Models;
using System.Collections.ObjectModel;

namespace NativeAOTDependencyHelper.ViewModels;

/// <summary>
/// Wrapper of all <see cref="NuGetPackageInfo"/> data we need to display in UI.
/// </summary>
public partial class NuGetPackageViewModel(NuGetPackageInfo _packageInfo, int _totalChecks) : ObservableObject
{
    public NuGetPackageInfo Info { get; } = _packageInfo;

    // TODO: Summarize the number of checks/total and stuff here from MainViewModel
    public ObservableCollection<ReportItem> ReportItems { get; } = new();

    public ObservableCollection<AOTCheckItem> CheckItems { get; } = new();

    public bool HasAnyFailedChecks => CheckItems.Any(check => check.Status != CheckStatus.Passed);

    public PackageCheckSummary CheckSummary { 
        get
        {
            if (PassedChecks == TotalChecks)
            {
                return PackageCheckSummary.AllPassed;
            }
            else if (PassedChecks == 0)
            {
                return PackageCheckSummary.NonePassed;
            }

            return PackageCheckSummary.SomePassed;
        } 
    }

    /// <summary>
    /// Total reports + checks (as checks are reports)
    /// </summary>
    [ObservableProperty]
    public partial int TotalReports { get; set; } = _totalChecks;

    /// <summary>
    /// Number of reports + checks currently completed
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAnyFailedChecks))]
    [NotifyPropertyChangedFor(nameof(ErroredChecks))]
    [NotifyPropertyChangedFor(nameof(PassedChecks))]
    [NotifyPropertyChangedFor(nameof(TotalChecks))]
    [NotifyPropertyChangedFor(nameof(CheckSummary))]
    public partial int ReportsCompleted { get; set; }

    /// <summary>
    /// Number of checks with errors.
    /// </summary>
    public int ErroredChecks => CheckItems.Count(check => check.Status == CheckStatus.Error) + ReportItems.Count(report => report.ProcessingError != null);

    /// <summary>
    /// Number of passed checks.
    /// </summary>
    public int PassedChecks => CheckItems.Count(check => check.Status == CheckStatus.Passed);

    public int TotalChecks => CheckItems.Count;

    [ObservableProperty]
    public partial PackageLoadStatus LoadStatus { get; set; } = PackageLoadStatus.Loading;

    public List<string> ProcessingErrors { get; } = new();

    /// <summary>
    /// Gets or sets the number of GitHub issues found with the text "AOT".
    /// TODO: We should have the status/links for those, expand once we hook into the data source there...
    /// </summary>
    [ObservableProperty]
    public partial int? NumberOfAOTIssues { get; set; }
}

public enum PackageCheckSummary
{
    NonePassed,
    SomePassed,
    AllPassed,
}
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

    public bool HasAnyFailedChecks => CheckItems.Any(check => !check.HasPassed);

    [ObservableProperty]
    private int _totalReports = _totalChecks;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAnyFailedChecks))]
    private int _reportsCompleted;


    
    // TODO: Move the below to individual reports... ( as needed )
    //// --- From NuGet.org ---

    /// <summary>
    /// Gets or sets if this package was found on NuGet.org.
    /// </summary>
    [ObservableProperty]
    private bool _isAvailableOnNuget;

    [ObservableProperty]
    private string? _latestNuGetVersion;

    [ObservableProperty]
    private string? _projectWebsite;

    [ObservableProperty]
    private string? _sourceRepositoryUrl;

    [ObservableProperty]
    private string? _registrationUrl;

    //// --- From GitHub.com ---

    /// <summary>
    /// Gets or sets if this package's code was found on GitHub.com
    /// </summary>
    [ObservableProperty]
    private bool _isAvailableOnGitHub;

    [ObservableProperty]
    private string? _lastCommitDate;

    /// <summary>
    /// Gets or sets whether we found the <code><IsAotCompatible>true</IsAotCompatible></code> flag within the source code (props or csprojs), null if we're searching or can't determine.
    /// TODO: We may want some better management of the state of this search, etc... here? As well as the search results, i.e. link to where the code was found.
    /// </summary>
    [ObservableProperty]
    private bool? _isAotCompatibleFlagFound;

    /// <summary>
    /// Gets or sets the number of GitHub issues found with the text "AOT".
    /// TODO: We should have the status/links for those, expand once we hook into the data source there...
    /// </summary>
    [ObservableProperty]
    private int? _numberOfAOTIssues;


}

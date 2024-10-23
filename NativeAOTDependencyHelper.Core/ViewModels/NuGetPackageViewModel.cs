using CommunityToolkit.Mvvm.ComponentModel;
using NativeAOTDependencyHelper.Core.JsonModels;

namespace NativeAOTDependencyHelper.Core.ViewModels;

/// <summary>
/// Wrapper of all NuGetPackage data we need to display in UI.
/// TODO: We should figure out if we want more separation here in our projects later between the core data gathering and what we need more specifically for the ViewModel layer...
/// </summary>
public partial class NuGetPackageViewModel(string parentProjectPath,
                                           string name,
                                           string framework,
                                           string requestedVersion,
                                           string resolvedVersion) : ObservableObject
{
    public string ParentProjectPath { get; } = parentProjectPath;

    public string Name { get; } = name;

    public string Framework { get; } = framework;

    public string RequestedVersion { get; } = requestedVersion;

    public string ResolvedVersion { get; } = resolvedVersion;

    public bool IsTransitive { get; set; }

    //// --- From NuGet.org ---

    /// <summary>
    /// Gets or sets if this package was found on NuGet.org.
    /// </summary>
    [ObservableProperty]
    private bool _isAvailableOnNuget;

    [ObservableProperty]
    private string? _latestNuGetVersion;

    [ObservableProperty]
    private string? _lastUploadDate; // TODO: We should probably use better typing here

    [ObservableProperty]
    private string? _projectWebsite;

    [ObservableProperty]
    private string? _license;

    [ObservableProperty]
    private string? _sourceRepositoryUrl;

    //// --- From GitHub.com ---

    /// <summary>
    /// Gets or sets if this package's code was found on GitHub.com
    /// </summary>
    [ObservableProperty]
    private bool _isAvailableOnGitHub;

    [ObservableProperty]
    private string? _lastCommitDate;

    /// <summary>
    /// Gets or sets whether we found the <code><IsAotComaptible>true</IsAotComaptible></code> flag within the source code (props or csprojs), null if we're searching or can't determine.
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

    // TODO: Would be nice if we can stream the Json deserialization and then process things by package one-by-one here too?
    public static IEnumerable<NuGetPackageViewModel> FromJsonModels(DotnetPackageList packageList)
    {
        foreach (var project in packageList.Projects)
        {
            foreach (var framework in project.Frameworks)
            {
                foreach (var package in framework.TopLevelPackages)
                {
                    yield return new NuGetPackageViewModel(
                        parentProjectPath: project.Path,
                        name: package.Id,
                        framework: framework.Framework,
                        requestedVersion: package.RequestedVersion,
                        resolvedVersion: package.ResolvedVersion);
                }

                foreach (var package in framework.TransitivePackages)
                {
                    yield return new NuGetPackageViewModel(
                        parentProjectPath: project.Path,
                        name: package.Id,
                        framework: framework.Framework,
                        requestedVersion: package.RequestedVersion,
                        resolvedVersion: package.ResolvedVersion)
                    {
                        IsTransitive = true
                    };
                }
            }
        }
    }
}

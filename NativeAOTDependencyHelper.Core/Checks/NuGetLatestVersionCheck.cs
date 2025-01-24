// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.Core.Sources;
using System.Text;

namespace NativeAOTDependencyHelper.Core.Checks;

/// <summary>
/// Checks if you're on the latest version of the package.
/// </summary>
/// <param name="_orchestrator"><see cref="TaskOrchestrator"/></param>
/// <param name="_nugetSource"><see cref="NuGetDataSource"/></param>
public class NuGetLatestVersionCheck(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource) : IReportItemProvider
{
    public string Name => "NuGet Package Latest";

    public ReportCategory Category => ReportCategory.Health;

    public int SortOrder => 3;

    public string Description => "Are you referencing the latest version available for the NuGet package?";

    public ReportType Type => ReportType.Check;

    public async Task<ReportItem?> ProcessPackage(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package, cancellationToken);

        bool found = false;
        string? latest = null;
        StringBuilder sb = new();

        if (packageMetadata?.Items.LastOrDefault() is RegistrationListings registrationList)
        {
            // TODO: We probably want to look for the latest stable release vs. flagging pre-releases
            // On the same line, if they're using the latest pre-release that shouldn't get flagged.
            latest = registrationList.Upper;
            foreach (var registration in registrationList.Items)
            {
                if (cancellationToken.IsCancellationRequested) return null;
                if (registration.CatalogEntry.Version == latest)
                {
                    // Check each project's version                    
                    foreach (var project in package.ProjectReferences)
                    {
                        // TODO: We probably have to do something with ResolvedVersion as well,
                        // at least in the case where there are no requested versions (i.e. pure transitive across all projects)
                        if (!string.IsNullOrWhiteSpace(project.RequestedVersion)
                            && project.RequestedVersion != latest)
                        {
                            found = true;

                            sb.AppendLine($"{project.Name} using {project.RequestedVersion} - latest is {latest}");
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            return new AOTCheckItem(this, CheckStatus.Error, "Could not read package registration data.");
        }

        if (!found)
        {
            return new AOTCheckItem(this, CheckStatus.Passed, "All projects using latest version");
        }

        return new AOTCheckItem(this, CheckStatus.Warning, sb.ToString());
    }
}

// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Checks;

public class NuGetRecentlyUpdatedCheck(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource) : IReportItemProvider
{
    /// <summary>
    /// Gets how many months old a package must be within to be considered recently updated.
    /// </summary>
    private const int NumberOfMonthsToBeRecentlyUpdated = 12;

    public string Name => "NuGet Package Up-to-date";

    public ReportCategory Category => ReportCategory.Health;

    public int SortOrder => 5;

    public string Description => "Has this package been updated in the past 12 months?";

    public ReportType Type => ReportType.Check;

    public async Task<ReportItem?> ProcessPackage(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package, cancellationToken);

        bool found = false;
        bool isRecent = false;
        DateTime? publishedDate = null;

        if (packageMetadata?.Items.LastOrDefault() is RegistrationListings registrationList)
        {
            var latest = registrationList.Upper;
            foreach (var registration in registrationList.Items)
            {
                if (cancellationToken.IsCancellationRequested) return null;
                if (registration.CatalogEntry.Version == latest)
                {
                    found = true;
                    publishedDate = registration.CatalogEntry.Published;
                    // Has the package been updated in the last year?
                    isRecent = publishedDate > DateTime.Now.AddMonths(-NumberOfMonthsToBeRecentlyUpdated);
                    break;
                }
            }
        }

        if (!found)
        {
            return new AOTCheckItem(this, CheckStatus.Unavailable, "Could not find latest package details");
        }

        return new AOTCheckItem(this, isRecent ? CheckStatus.Passed : CheckStatus.Warning, $"Package last updated: {publishedDate}");
    }
}

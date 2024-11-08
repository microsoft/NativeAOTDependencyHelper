using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.JsonModels;
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

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package);

        bool found = false;
        bool isRecent = false;
        DateTime? publishedDate = null;

        if (packageMetadata?.Items.LastOrDefault() is RegistrationListings registrationList)
        {
            var latest = registrationList.Upper;
            foreach (var registration in registrationList.Items)
            {
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
            return new AOTCheckItem(this, CheckStatus.Error, "Could not find latest package details");
        }

        return new AOTCheckItem(this, isRecent ? CheckStatus.Passed : CheckStatus.Warning, $"Package last updated: {publishedDate}");
    }
}

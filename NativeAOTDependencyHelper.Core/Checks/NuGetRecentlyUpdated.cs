using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Checks;

public class NuGetRecentlyUpdated(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource) : IReportItemProvider
{
    /// <summary>
    /// Gets how many months old a package must be within to be considered recently updated.
    /// </summary>
    private const int NumberOfMonthsToBeRecentlyUpdated = 12;

    public string Name => "Nuget Package Up-to-date";

    public int SortOrder => 5;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        // TODO: Thinking that since we'll be having multiple checks calling for the package info
        //       We should have the base orchestrator that does scheduling be an intermediary
        //       It can get the specific data source info and cache it so each source doesn't need to worry about it?
        // await _orchestrator.GetDataSourceInfo<NuGetPackageMetadata>(package, _nugetSource)
        // And then the data source has a GetInfoForPackageAsync<T> in the interface called by the orchestrator, and it'll cache that result, so the above call will either make the request to the data source
        // or it'll just returned the cached result for the existing package.

        // await _nugetSource.GetNuGetInfoAsync(package);
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
            return new AOTCheckItem(this, false, "Could not find latest package details");
        }

        return new AOTCheckItem(this, isRecent, $"Package last updated: {publishedDate}");
    }
}

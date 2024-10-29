using Microsoft.Extensions.DependencyInjection;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.Core.Sources;

namespace NativeAOTDependencyHelper.Core.Checks;

public class NuGetRecentlyUpdatedCheck(TaskOrchestrator _orchestrator, [FromKeyedServices(NuGetDataSource.ServiceId)] IDataSource _nugetSource) : IReportItemProvider
{
    /// <summary>
    /// Gets how many months old a package must be within to be considered recently updated.
    /// </summary>
    private const int NumberOfMonthsToBeRecentlyUpdated = 12;

    public string Name => "Nuget Package Up-to-date";

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
            return new AOTCheckItem(this, false, "Could not find latest package details");
        }

        return new AOTCheckItem(this, isRecent, $"Package last updated: {publishedDate}");
    }
}

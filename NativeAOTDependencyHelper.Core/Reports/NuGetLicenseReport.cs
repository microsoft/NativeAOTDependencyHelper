using Microsoft.Extensions.DependencyInjection;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.Core.Sources;

namespace NativeAOTDependencyHelper.Core.Reports;

public class NuGetLicenseReport(TaskOrchestrator _orchestrator, [FromKeyedServices(NuGetDataSource.ServiceId)] IDataSource _nugetSource) : IReportItemProvider
{
    public string Name => "Package Licence";

    public int SortOrder => 10;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package);

        if (packageMetadata?.Items.LastOrDefault() is RegistrationListings registrationList)
        {
            var latest = registrationList.Upper;
            foreach (var registration in registrationList.Items)
            {
                if (registration.CatalogEntry.Version == latest)
                {
                    // TODO: Do we care if this fails?
                    Uri.TryCreate(registration.CatalogEntry.LicenseUrl, UriKind.Absolute, out var licenseUri);

                    return new ReportItem(this, $"License: {registration.CatalogEntry.LicenseExpression}", licenseUri);
                }
            }
        }

        return new ReportItem(this, "Could not find license information.");
    }
}

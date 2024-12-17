using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Reports;

public class NuGetLicenseReport(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource) : IReportItemProvider
{
    public string Name => "Package License";

    public ReportCategory Category => ReportCategory.Informational;

    public int SortOrder => 10;

    public string Description => "Displays the license of the dependency";

    public ReportType Type => ReportType.Report;

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

                    if (String.IsNullOrEmpty(registration.CatalogEntry.LicenseExpression))
                    {
                        if (licenseUri != null) return new ReportItem(this, "Click to view license details", licenseUri);
                        else return new ReportItem(this, "License information not available", licenseUri);
                    }

                    return new ReportItem(this, $"License: {registration.CatalogEntry.LicenseExpression}", licenseUri);
                }
            }
        }

        return new ReportItem(this, "Could not find license information.");
    }
}

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
    public string Name => "Nuget Package Latest";

    public int SortOrder => 3;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package);

        bool found = false;
        string? latest = null;
        StringBuilder sb = new();

        if (packageMetadata?.Items.LastOrDefault() is RegistrationListings registrationList)
        {
            latest = registrationList.Upper;
            foreach (var registration in registrationList.Items)
            {
                if (registration.CatalogEntry.Version == latest)
                {                    
                    // Check each project's version                    
                    foreach (var project in package.ProjectReferences)
                    {
                        if (project.RequestedVersion != latest)
                        {
                            found = true;

                            sb.AppendLine($"{project.Name} using {project.RequestedVersion} - latest is {latest}");
                        }
                    }
                    break;
                }
            }
        }

        if (!found)
        {
            return new AOTCheckItem(this, true, "All projects using latest version");
        }

        return new AOTCheckItem(this, false, sb.ToString());
    }
}

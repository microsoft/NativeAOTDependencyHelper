using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Sources;

namespace NativeAOTDependencyHelper.Core.Checks;

public class NuGetRecentlyUpdated(NuGetDataSource _nugetSource) : IReportItemProvider
{
    public string Name => "Nuget Package Up-to-date";

    public int SortOrder => 5;

    public Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        // TODO: Thinking that since we'll be having multiple checks calling for the package info
        //       We should have the base orchestrator that does scheduling be an intermediary
        //       It can get the specific data source info and cache it so each source doesn't need to worry about it?
        // await _orchestrator.GetDataSourceInfo<NuGetPackageMetadata>(package, _nugetSource)
        // And then the data source has a GetInfoForPackageAsync<T> in the interface called by the orchestrator, and it'll cache that result, so the above call will either make the request to the data source
        // or it'll just returned the cached result for the existing package.

        // await _nugetSource.GetNuGetInfoAsync(package);
        throw new NotImplementedException();
    }
}

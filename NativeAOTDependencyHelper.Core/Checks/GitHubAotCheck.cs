using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Checks
{
    public class GitHubAotCheck(TaskOrchestrator _orchestrator, IDataSource<GitHubCodeSearchResult> _gitHubSource) : IReportItemProvider
    {
        public string Name => "GitHub AOT Tag";

        public int SortOrder => 10;

        public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
        {
            var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<GitHubCodeSearchResult>(_gitHubSource, package);
            
            return new ReportItem(this, "Could not find GitHub info.");
        }
    }
}

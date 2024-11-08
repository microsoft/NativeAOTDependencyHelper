using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Checks;

public class GitHubAotIssuesCheck(TaskOrchestrator _orchestrator, IDataSource<GitHubIssueSearchResult> _gitHubSource) : IReportItemProvider
{
    public string Name => "AOT-related GitHub issues";

    public int SortOrder => 10;

    public ReportCategory Category => ReportCategory.AOTCompatibility;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<GitHubIssueSearchResult>(_gitHubSource, package);
        if (packageMetadata == null || packageMetadata.TotalItems == 0) return new ReportItem(this, "No AOT-related open issues found", null);
        return new ReportItem(this, $"{packageMetadata.TotalItems} open issues found. Click to view list of issues", packageMetadata.IssuesQuery);
    }
}

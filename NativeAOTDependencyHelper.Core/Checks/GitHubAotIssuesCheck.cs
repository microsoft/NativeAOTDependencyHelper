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
        if (packageMetadata == null) return new ReportItem(this, "No repository data given to search for AOT tag", null);
        if (packageMetadata.Error != null) return new ReportItem(this, "Error performing GitHub issues search for query " + packageMetadata.IssuesQuery, null, "Error performing GitHub issues search request: " + packageMetadata.Error);
        if (packageMetadata.TotalItems == 0) return new ReportItem(this, "No open issues found. Click to open search query.", packageMetadata.IssuesQuery);
        return new ReportItem(this, $"{packageMetadata.TotalItems} open issues found. Click to view list of issues", packageMetadata.IssuesQuery);
    }
}

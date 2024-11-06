using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeAOTDependencyHelper.Core.Checks;
public class GitHubAotIssuesCheck(TaskOrchestrator _orchestrator, IDataSource<GitHubIssueSearchResult> _gitHubSource) : IReportItemProvider
{
    public string Name => "AOT-related GitHub issues";

    public int SortOrder => 10;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<GitHubIssueSearchResult>(_gitHubSource, package);
        if (packageMetadata == null || packageMetadata.TotalItems == 0) return new ReportItem(this, "No AOT-related issues found", null);
        return new ReportItem(this, $"{packageMetadata.TotalItems} issues found. Click to view list of issues", packageMetadata.IssuesQuery);
    }
}

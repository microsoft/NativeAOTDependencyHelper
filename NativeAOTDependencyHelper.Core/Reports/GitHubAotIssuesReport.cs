// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Reports;

public class GitHubAotIssuesReport(TaskOrchestrator _orchestrator, IDataSource<GitHubIssueSearchResult> _gitHubSource) : IReportItemProvider
{
    public string Name => "AOT-related GitHub issues";

    public int SortOrder => 10;

    public ReportCategory Category => ReportCategory.AOTCompatibility;

    public ReportType Type => ReportType.Report;

    public string Description => "Searches GitHub (if available) for open issues containing 'AOT'";

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync(_gitHubSource, package);
        if (packageMetadata == null) return new ReportItem(this, "No repository data given to search for AOT tag", null);
        if (packageMetadata.Error != null) return new ReportItem(this, "Error performing GitHub issues search for query " + packageMetadata.IssuesQuery, null, "Error performing GitHub issues search request: " + packageMetadata.Error);
        if (packageMetadata.TotalItems == 0) return new ReportItem(this, "No open issues found. Click to open search query.", packageMetadata.IssuesQuery);
        return new ReportItem(this, $"{packageMetadata.TotalItems} open issues found. Click to view list of issues", packageMetadata.IssuesQuery);
    }
}

using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.JsonModels;

public class GitHubIssueSearchResult
{
    public int TotalItems { get; private set; }
    public Uri? IssuesQuery { get; private set; }
    public CheckStatus checkStatus { get; set; } = CheckStatus.Unavailable;

    public GitHubIssueSearchResult(int totalItems, Uri? issuesQuery)
    {
        TotalItems = totalItems;
        IssuesQuery = issuesQuery;
    }
}
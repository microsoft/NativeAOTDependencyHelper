using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.JsonModels;

public class GitHubIssueSearchResult
{
    public int TotalItems { get; private set; }
    public Uri? IssuesQuery { get; private set; }
    public CheckStatus checkStatus { get; set; } = CheckStatus.Unavailable;

    public string? Error { get; set; }

    public GitHubIssueSearchResult(int totalItems, Uri? issuesQuery, string? error = null)
    {
        TotalItems = totalItems;
        IssuesQuery = issuesQuery;
        if (error != null)
        {
            Error = error;
            checkStatus = CheckStatus.Error;
        }
    }
}
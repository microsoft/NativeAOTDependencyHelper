namespace NativeAOTDependencyHelper.Core.JsonModels;
public class GitHubIssueSearchResult
{
    public int TotalItems { get; private set; }
    public Uri? IssuesQuery { get; private set; }

    public GitHubIssueSearchResult(int totalItems, Uri? issuesQuery)
    {
        TotalItems = totalItems;
        IssuesQuery = issuesQuery;
    }
}
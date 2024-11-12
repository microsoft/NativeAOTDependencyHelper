using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using Nito.AsyncEx;
using Octokit;

namespace NativeAOTDependencyHelper.Core.Sources;

public class GitHubIssueSearchDataSource(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource, GitHubOAuthService gitHubOAuthService) : IDataSource<GitHubIssueSearchResult?>
{
    public string Name => "GitHub AOT Issue Search";

    public string Description => "Searches GitHub repo for issues related to AOT";

    public bool IsInitialized { get; private set; }

    private GitHubClient? _gitHubClient;

    private const string gitHubUrl = "https://github.com/";
    
    private readonly AsyncLock _mutex = new();

    public async Task<bool> InitializeAsync()
    {
        _gitHubClient = await gitHubOAuthService?.StartAuthRequest();
        IsInitialized = _gitHubClient != null;
        return _gitHubClient != null;
    }

    public async Task<GitHubIssueSearchResult?> GetInfoForPackageAsync(NuGetPackageInfo package)
    {
        using (await _mutex.LockAsync())
        {
            // We mutex the datasource and artificially delay here as GH API is rate limited 5000/hr - https://docs.github.com/rest/using-the-rest-api/rate-limits-for-the-rest-api
            await Task.Delay(1000); // We could probably lessen this, but for now leaving as 1000 (over 720) in case we add more checks elsewhere, we should probably manage this in the AuthService centrally or something?

            var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package);
            if (packageMetadata?.RepositoryUrl == null || !packageMetadata.RepositoryUrl.Contains(gitHubUrl)) return null;
            // Parsing repo path
            var repoPath = packageMetadata?.RepositoryUrl.Replace(gitHubUrl, "");
            // Remove .git url suffix since this doesn't work in search
            if (repoPath?.EndsWith(".git") == true) repoPath = repoPath.Replace(".git", "");

            var request = new SearchIssuesRequest("aot")
            {
                Repos = new RepositoryCollection { repoPath },
                Type = IssueTypeQualifier.Issue,
                State = ItemState.Open
            };
            try
            {
                var result = await _gitHubClient?.Search.SearchIssues(request);
                if (result == null || result.TotalCount == 0) return null;
                var queryUri = new Uri($"{packageMetadata?.RepositoryUrl}/issues?q=type%3Aissue%20state%3Aopen%20aot");
                return new GitHubIssueSearchResult(result.TotalCount, queryUri);
            } catch (Exception e)
            {
                return new GitHubIssueSearchResult(0, null, e.Message);
            }
        }
    }
}

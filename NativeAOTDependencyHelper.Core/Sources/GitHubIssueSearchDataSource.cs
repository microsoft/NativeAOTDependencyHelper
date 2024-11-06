using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.Core.Services.GitHubOAuth;
using Octokit;

namespace NativeAOTDependencyHelper.Core.Sources;
public class GitHubIssueSearchDataSource(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource, GitHubOAuthService gitHubOAuthService) : IDataSource<GitHubIssueSearchResult?>
{
    public string Name => "GitHub AOT Issue Search";

    public string Description => "Searches GitHub repo for issues related to AOT";

    public bool IsInitialized { get; private set; }

    private GitHubClient? _gitHubClient;

    public async Task<bool> InitializeAsync()
    {
        _gitHubClient = await gitHubOAuthService?.StartAuthRequest();
        return _gitHubClient != null;
    }

    public async Task<GitHubIssueSearchResult?> GetInfoForPackageAsync(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package);
        if (packageMetadata?.RepositoryUrl == null) return null;
        var repoPath = packageMetadata?.RepositoryUrl.Replace("https://github.com/", "");

        var request = new SearchIssuesRequest("aot")
        {
            Repos = new RepositoryCollection { repoPath },
            Type = IssueTypeQualifier.Issue
        };
        try
        {
            var result = await _gitHubClient?.Search.SearchIssues(request);
            if (result == null || result.TotalCount == 0) return null;
            var queryUri = new Uri($"{packageMetadata?.RepositoryUrl}/issues?q=type%3Aissue%20aot");
            return new GitHubIssueSearchResult(result.TotalCount, queryUri);
        } catch
        {
            // TODO: Display error for search 
            return null;
        }
    }
}

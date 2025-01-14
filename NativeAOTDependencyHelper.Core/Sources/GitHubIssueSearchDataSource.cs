// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using Nito.AsyncEx;
using Octokit;

namespace NativeAOTDependencyHelper.Core.Sources;

public class GitHubIssueSearchDataSource(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource, GitHubOAuthService gitHubOAuthService, ILogger _logger) : IDataSource<GitHubIssueSearchResult?>
{
    public string Name => "GitHub AOT Issue Search";

    public string Description => "Searches GitHub repo for open issues related to AOT";

    public bool IsInitialized { get; private set; }

    private GitHubClient? _gitHubClient;

    private const string gitHubUrl = "https://github.com/";

    private readonly AsyncLock _mutex = new();

    public async Task<bool> InitializeAsync()
    {
        _gitHubClient = await gitHubOAuthService?.StartAuthRequest();
        IsInitialized = _gitHubClient != null;
        if (!IsInitialized)
        {
            _logger.Warning("GitHub Issue Source hasn't authenticated to GitHub");
        }
        else
        {
            _logger.Information("GitHub Issue Source Authorized for GitHub");
        }
        return _gitHubClient != null;
    }

    public async Task<GitHubIssueSearchResult?> GetInfoForPackageAsync(NuGetPackageInfo package)
    {
        using (await _mutex.LockAsync())
        {
            // We mutex the datasource and artificially delay here as GH API is rate limited 5000/hr - https://docs.github.com/rest/using-the-rest-api/rate-limits-for-the-rest-api
            await Task.Delay(1000); // We could probably lessen this, but for now leaving as 1000 (over 720) in case we add more checks elsewhere, we should probably manage this in the AuthService centrally or something?

            NuGetPackageRegistration? packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync(_nugetSource, package);
            if (packageMetadata?.RepositoryUrl == null || !packageMetadata.RepositoryUrl.Contains(gitHubUrl)) return null;
            // Parsing repo path. Remove .git url suffix since this doesn't work in search

            var repoUrl = packageMetadata?.RepositoryUrl.Replace(".git", "");
            var repoPath = repoUrl?.Replace(gitHubUrl, "");

            var request = new SearchIssuesRequest("aot")
            {
                Repos = new RepositoryCollection { repoPath },
                Type = IssueTypeQualifier.Issue,
                State = ItemState.Open
            };

            var result = await _gitHubClient?.Search.SearchIssues(request);
            if (result == null || result.TotalCount == 0) return null;
            var queryUri = new Uri($"{repoUrl}/issues?q=type%3Aissue%20state%3Aopen%20aot");
            try
            {
                return new GitHubIssueSearchResult(result.TotalCount, queryUri);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error searching GitHub Issues for {package.Name}");
                return new GitHubIssueSearchResult(0, queryUri, e.Message);
            }
        }
    }
}

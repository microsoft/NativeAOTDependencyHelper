// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using Nito.AsyncEx;
using Octokit;
using System.Net.Http.Json;
using System.Xml.Linq;
using CheckStatus = NativeAOTDependencyHelper.Core.Models.CheckStatus;

namespace NativeAOTDependencyHelper.Core.Sources;

/**
 * Performs a code search request to see if the project contains definitions for the <IsAotCompatible> flag
 */
public class GitHubAotCompatibleCodeSearchDataSource(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource, GitHubOAuthService gitHubOAuthService, ILogger _logger) : IDataSource<GitHubCodeSearchResult?>
{
    public string Name => "GitHub IsAotCompatible Code Search";

    public string Description => "Retrieves information about the package source from its GitHub repository, if available. Looks for the IsAotCompatible flag.";

    public bool IsInitialized { get; private set; }

    private GitHubClient? _githubClient;

    private static HttpClient _httpClient = new();

    private readonly AsyncLock _mutex = new();

    public async Task<bool> InitializeAsync()
    {
        _githubClient = await gitHubOAuthService?.StartAuthRequest();
        IsInitialized = _githubClient != null;
        if (!IsInitialized)
        {
            _logger.Warning("GitHub Code Source hasn't authenticated to GitHub");
        }
        else
        {
            _logger.Information("GitHub Code Source Authorized for GitHub");
        }
        return _githubClient != null;
    }

    public async Task<GitHubCodeSearchResult?> GetInfoForPackageAsync(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        SearchCodeResult result;

        try
        {
            using (await _mutex.LockAsync())
            {
                // We mutex the datasource and artificially delay here as Code Search API is rate limited 10/min - https://docs.github.com/rest/search/search
                await Task.Delay(6250); // Technically, 6000, but adding a bit of buffer.

                // GitHub search code request to fetch source file url that contains <IsAotCompatible> tag
                cancellationToken.ThrowIfCancellationRequested();
                NuGetPackageRegistration? packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync(_nugetSource, package, cancellationToken);
                if (packageMetadata?.RepositoryUrl == null) return null;
                var repoPath = packageMetadata?.RepositoryUrl.Replace("https://github.com/", "");
                var request = new SearchCodeRequest("<IsAotCompatible")
                {
                    In = new[] { CodeInQualifier.File, CodeInQualifier.Path },
                    Language = Language.Xml,
                    Repos = new RepositoryCollection { repoPath }
                };

                result = await _githubClient?.Search.SearchCode(request);
                if (result == null || result.TotalCount == 0) return null;

            }

                // Fetching source file from url parsed in GitHub code search response
            var gitSource = result.Items[0].Url;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AOT Compatibility Tool");

            // Parsing XML tags to find <IsAotCompatible> tag
            cancellationToken.ThrowIfCancellationRequested();
            var repoInfo = await _httpClient.GetFromJsonAsync<GitHubCodeSearchResult>(gitSource); // TODO: Check if this will rate limit us too?
            var sourceFile = await _httpClient.GetAsync(repoInfo.DownloadUrl);
            var sourceXml = await sourceFile.Content.ReadAsStreamAsync();
            XDocument doc = XDocument.Load(sourceXml);

            var aotTag = doc.Descendants("PropertyGroup")
                .Elements("IsAotCompatible")
                .FirstOrDefault();

            if (aotTag != null)
            {
                repoInfo.IsAotCompatible = aotTag != null && aotTag.Value == "true";
                repoInfo.CheckStatus = CheckStatus.Passed;
            }
            else
            {
                repoInfo.CheckStatus = CheckStatus.Warning;
            }
            return repoInfo;
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Error searching GitHub Code for {package.Name}");
            return new GitHubCodeSearchResult
            {
                CheckStatus = CheckStatus.Error,
                Error = e.Message
            };
        }
    }

}

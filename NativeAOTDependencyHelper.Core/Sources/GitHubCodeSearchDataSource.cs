using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Services.GitHubOAuth;
using Octokit;
using System.Diagnostics;
using NativeAOTDependencyHelper.Core.Services;
using System.Xml.Linq;
using System.Net.Http.Json;

namespace NativeAOTDependencyHelper.Core.Sources;

public class GitHubCodeSearchDataSource(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource, GitHubOAuthService gitHubOAuthService) : IDataSource<GitHubCodeSearchResult?>
{
    public string Name => "GitHub Search Information";

    public string Description => "Retrieves information about the package from its GitHub repository";

    public bool IsInitialized { get; private set; }

    private GitHubClient? _githubClient;

    private static HttpClient _httpClient = new();

    public async Task<bool> InitializeAsync()
    {
        if (_githubClient != null) return true;

        _githubClient = await gitHubOAuthService?.StartAuthRequest();
        return _githubClient != null;
    }

    public async Task<GitHubCodeSearchResult?> GetInfoForPackageAsync(NuGetPackageInfo package)
    {
        // GitHub search code request to fetch source file url that contains <IsAotCompatible> tag
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package);
        if (packageMetadata?.RepositoryUrl == null) return null;
        var repoPath = packageMetadata?.RepositoryUrl.Replace("https://github.com/", "");
        var request = new SearchCodeRequest("<IsAotCompatible>")
        {
            In = new[] { CodeInQualifier.File, CodeInQualifier.Path },
            Language = Language.Xml,
            Repos = new RepositoryCollection { repoPath }
        };

        var result = await _githubClient?.Search.SearchCode(request);
        if (result == null || result.TotalCount == 0) return null;

        // Fetching source file from url parsed in GitHub code search response
        var gitSource = result.Items[0].Url;
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "AOT Compatibility Tool");

        try
        {
            // Parsing XML tags to find <IsAotCompatible> tag
            var repoInfo = await _httpClient.GetFromJsonAsync<GitHubCodeSearchResult>(gitSource);
            var sourceFile = await _httpClient.GetAsync(repoInfo.DownloadUrl);
            var sourceXml = await sourceFile.Content.ReadAsStreamAsync();
            XDocument doc = XDocument.Load(sourceXml);
            var aotTag = doc.Descendants("PropertyGroup")
                .Elements("IsAotCompatible")
                .FirstOrDefault();

            if (aotTag != null) repoInfo.IsAotCompatible = aotTag != null && aotTag.Value == "true";
            return repoInfo;
        }
        catch (HttpRequestException e)
        {
            Debug.WriteLine(e.StatusCode + ": " + e.Message);
        }

        return null;
    }

}

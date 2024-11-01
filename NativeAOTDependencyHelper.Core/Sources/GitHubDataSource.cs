using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Services.GitHubOAuth;
using Octokit;
using System.Diagnostics;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Sources
{
    public class GitHubDataSource(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource, GitHubOAuthService gitHubOAuthService) : IDataSource<GitHubCodeSearchResult>
    {
        public string Name => "GitHub Search Information";

        public string Description => "Retrieves information about the package from its GitHub repository";

        public bool IsInitialized { get; private set; }

        private GitHubClient? _githubClient;

        public async Task<bool> InitializeAsync()
        {
            _githubClient = await gitHubOAuthService?.StartAuthRequest();
            return _githubClient != null;
        }

        public async Task<GitHubCodeSearchResult?> GetInfoForPackageAsync(NuGetPackageInfo package)
        {
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
            Debug.WriteLine(result);
            // STUB
            return new GitHubCodeSearchResult();
        }
 
    }
}

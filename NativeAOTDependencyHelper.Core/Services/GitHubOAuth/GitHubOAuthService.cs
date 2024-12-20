// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Nito.AsyncEx;
using Octokit;

namespace NativeAOTDependencyHelper.Core.Services;

public class GitHubOAuthService(CredentialManager credentialManager)
{
    const string clientName = "NativeAOTDependencyHelper";

    private GitHubClient? _gitHubClient;

    private readonly AsyncLock _mutex = new();

    public async Task<GitHubClient?> StartAuthRequest()
    {
        using (await _mutex.LockAsync())
        {
            if (_gitHubClient != null) return _gitHubClient;

            _gitHubClient = new GitHubClient(new ProductHeaderValue(clientName))
            {
                Credentials = new Credentials(credentialManager.ReadCredential())
            };
            return _gitHubClient;
        }
    }

}

using Octokit;
using System.Diagnostics;
using System.Net;

namespace NativeAOTDependencyHelper.Core.Services.GitHubOAuth;

public class GitHubOAuthService
{
    const string clientName = "NativeAOTDependencyHelper";
    const string clientId = "clientId";
    const string clientSecret = "clientSecret";
    const string redirectUri = "http://localhost:5000/callback/";

    private string GetAuthorizationUrl()
    {
        return $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope=read:user";
    }

    public async Task<GitHubClient?> StartAuthRequest()
    {
        var authorizationUrl = GetAuthorizationUrl();
        var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        listener.Start();

        Process.Start(new ProcessStartInfo(authorizationUrl) { UseShellExecute = true });

        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"];

        var client = await CompleteGitHubOAuthFlowAsync(code);

        // Respond to the browser
        var response = context.Response;
        string responseString = "<html><body>You can close this window now.</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        listener.Stop();

        return client;
    }

    private async Task<GitHubClient?> CompleteGitHubOAuthFlowAsync(string code)
    {
        try
        {
            var client = await GetGitHubClient(code);
            var user = await client.User.Current();
            Debug.WriteLine($"Authenticated as {user.Login}");
            return client;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Authentication failed: {ex.Message}");
            return null;
        }
    }

    private async Task<GitHubClient> GetGitHubClient(string code)
    {
        var _client = new GitHubClient(new ProductHeaderValue(clientName));
        var token = await _client.Oauth.CreateAccessToken(new OauthTokenRequest(clientId, clientSecret, code));
        _client.Credentials = new Credentials(token.AccessToken);
        return _client;
    }
}

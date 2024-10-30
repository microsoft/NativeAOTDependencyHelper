using IdentityModel.OidcClient;
using Newtonsoft.Json.Linq;
using Octokit;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;
namespace NativeAOTDependencyHelper.Core.Services.GitHubOAuth
{
    public class GitHubOAuthService
    {
        const string clientId = "clientId";
        const string clientSecret = "clientSecret";
        // TODO: Figure out how to redirect user back to app and return auth token
        const string redirectUri = "http://localhost:5000/callback/";

        public string GetAuthorizationUrl()
        {
            return $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope=read:user";
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            });

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(content)["access_token"].ToString();
            return token;
        }

        public GitHubClient GetGitHubClient(string accessToken)
        {
            var client = new GitHubClient(new Octokit.ProductHeaderValue("NativeAOTDependencyHelper"))
            {
                Credentials = new Credentials(accessToken)
            };
            return client;
        }
    }
}

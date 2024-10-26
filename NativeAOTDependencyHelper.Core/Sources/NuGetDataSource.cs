using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using System.Net.Http.Json;

namespace NativeAOTDependencyHelper.Core.Sources;

public class NuGetDataSource(SolutionPackageIndex _packageIndex) : IDataSource
{
    public string Name => "NuGet.org Package Information";

    public string Description => "Retrieves information about package metadata from NuGet.org";

    private static string _registrationFileExt = "/index.json";

    // We're sharing this for all main calls within our source.
    // HttpClient lifecycle management best practices:
    // https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
    private static HttpClient _sharedHttpClient = new();

    /// <summary>
    /// We need to lookup the service url for calling our main nuget service.
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        using (var httpClient = new HttpClient())
        {
            // Get the list of all books
            var index = await _sharedHttpClient.GetFromJsonAsync<NuGetServiceIndex>("https://api.nuget.org/v3/index.json");

            if (index == null) return false;

            foreach (var service in index.Resources)
            {
                // Need to find the base Url for the Registration service for our subsequent package calls
                if (service.Type == "RegistrationsBaseUrl")
                {
                    httpClient.BaseAddress = new Uri(service.Id);
                    _sharedHttpClient = new HttpClient { BaseAddress = new Uri(service.Id) }; // Create a new HttpClient with the BaseAddress set
                    return true;
                }
            }

            return false;
        }
    }

    public async Task<NuGetPackageRegistration?> GetPackageRegistration(string packageId)
    {
        return await _sharedHttpClient.GetFromJsonAsync<NuGetPackageRegistration>(packageId.ToLower() + _registrationFileExt);
    }
}

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using System.Net.Http.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NativeAOTDependencyHelper.Core.Sources;

public class NuGetDataSource : IDataSource<NuGetPackageRegistration>
{
    // Note: We want this to be a static abstract on the IDataSource, but that doesn't work with services... so we'll just have this be our convention
    public const string ServiceId = "NuGetRegistration";

    public string Name => "NuGet.org Package Information";

    public string Description => "Retrieves information about package metadata from NuGet.org";

    private static Dictionary<String, String> _serviceTypeToUri = new();

    public bool IsInitialized { get; private set; }


    // We're sharing this for all main calls within our source.
    // HttpClient lifecycle management best practices:
    // https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
    private static HttpClient _sharedHttpClient = new();

    /// <summary>
    /// We need to lookup the service url for calling our main nuget service.
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        // Get service index
        var index = await _sharedHttpClient.GetFromJsonAsync<NuGetServiceIndex>("https://api.nuget.org/v3/index.json");

        if (index == null) return false;

        foreach (var service in index.Resources)
        {
            _serviceTypeToUri[service.Type] = service.Id;
        }
        IsInitialized = true;
        return false;
    }

    public async Task<NuGetPackageRegistration?> GetInfoForPackageAsync(NuGetPackageInfo package)
    {
        // Type = RegistrationsBaseUrl
        var registration = await GetRegistrationsForPackageAsync<NuGetPackageRegistration>(package);
        var version = registration?.Items?.FirstOrDefault()?.Upper;
        return await GetMetadataFromNuspec(registration, package.Name, version);
    }

    public async Task<NuGetPackageRegistration?> GetRegistrationsForPackageAsync<NuGetPackageRegistration>(NuGetPackageInfo package)
    {
        return await _sharedHttpClient.GetFromJsonAsync<NuGetPackageRegistration>(_serviceTypeToUri["RegistrationsBaseUrl"] + package.Name.ToLower() + "/index.json");
    }

    public async Task<NuGetPackageRegistration?> GetMetadataFromNuspec(NuGetPackageRegistration? registration, string packageId, string version)
    {
        // Type = PackageBaseAddress/3.0.0
        var response = await _sharedHttpClient.GetAsync(_serviceTypeToUri["PackageBaseAddress/3.0.0"] + packageId.ToLower() + "/" + version + "/" + packageId.ToLower() + ".nuspec");
        response.EnsureSuccessStatusCode();

        using (var nuspecStream = await response.Content.ReadAsStreamAsync())
        {
            XDocument doc = XDocument.Load(nuspecStream);
            var repository = from element in doc.Descendants()
                             where element.Name.LocalName == "repository"
                             select element;

            if (registration != null && repository != null)
            {
                registration.RepositoryUrl = repository?.FirstOrDefault()?.Attribute("url")?.Value;
            }
            return registration;
        }
    }
}

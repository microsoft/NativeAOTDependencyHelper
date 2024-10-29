﻿using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System.Net.Http.Json;

namespace NativeAOTDependencyHelper.Core.Sources;

public class NuGetDataSource : IDataSource
{
    // Note: We want this to be a static abstract on the IDataSource, but that doesn't work with services... so we'll just have this be our convention
    public const string ServiceId = "NuGetRegistration";

    public string Name => "NuGet.org Package Information";

    public string Description => "Retrieves information about package metadata from NuGet.org";

    public bool IsInitialized { get; private set; }

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

                    IsInitialized = true;
                    return true;
                }
            }

            return false;
        }
    }

    public Task<NuGetPackageRegistration?> GetInfoForPackageAsync<NuGetPackageRegistration>(NuGetPackageInfo package)
    {
        return _sharedHttpClient.GetFromJsonAsync<NuGetPackageRegistration>(package.Name.ToLower() + _registrationFileExt);
    }
}

// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace NativeAOTDependencyHelper.Core.Sources;

public class NuGetDataSource(ILogger _logger) : IDataSource<NuGetPackageRegistration>
{
    public string Name => "NuGet.org Package Information";

    public string Description => "Retrieves information about package metadata from NuGet.org";

    public bool IsInitialized { get; private set; }

    public string? PackageBaseAddress => _serviceTypeToUri.TryGetValue("PackageBaseAddress/3.0.0", out var value) ? value : null;

    public string? RegistrationsBaseUrl => _serviceTypeToUri.TryGetValue("RegistrationsBaseUrl", out var value) ? value : null;

    private static Dictionary<string, string> _serviceTypeToUri = new();

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

        if (index == null)
        {
            _logger.Warning("Issue initializing NuGet data source");
            return false;
        }

        foreach (var service in index.Resources)
        {
            _serviceTypeToUri[service.Type] = service.Id;
        }
        IsInitialized = true;
        _logger.Information("NuGet Data Source Initialized");
        return false;
    }

    public async Task<NuGetPackageRegistration?> GetInfoForPackageAsync(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        try
        {
            // Type = RegistrationsBaseUrl
            cancellationToken.ThrowIfCancellationRequested();
            var registration = await _sharedHttpClient.GetFromJsonAsync<NuGetPackageRegistration>($"{RegistrationsBaseUrl}{package.Name.ToLower()}/index.json", cancellationToken);
            var version = registration?.Items?.FirstOrDefault()?.Upper;
            return await GetMetadataFromNuspec(registration, package.Name, version!);
        }
        catch (OperationCanceledException e)
        {
            return new NuGetPackageRegistration
            {
                Id = package.Name,
                Error = e.Message
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Error retrieving NuGet package info for {package.Name}");
            return new NuGetPackageRegistration
            {
                Id = package.Name,
                Error = e.Message
            };
        }
    }

    private async Task<NuGetPackageRegistration?> GetMetadataFromNuspec(NuGetPackageRegistration? registration, string packageId, string version)
    {
        if (registration == null || PackageBaseAddress == null || version == null) return null;

        // Type = PackageBaseAddress/3.0.0
        var response = await _sharedHttpClient.GetAsync($"{PackageBaseAddress}{packageId.ToLower()}/{version}/{packageId.ToLower()}.nuspec");
        response.EnsureSuccessStatusCode();

        using var nuspecStream = await response.Content.ReadAsStreamAsync();
        registration.Metadata = XDocument.Load(nuspecStream);
        var repository = from element in registration.Metadata.Descendants()
                         where element.Name.LocalName == "repository"
                         select element;

        if (repository != null)
        {
            registration.RepositoryUrl = repository?.FirstOrDefault()?.Attribute("url")?.Value;
        }

        return registration;
    }
}

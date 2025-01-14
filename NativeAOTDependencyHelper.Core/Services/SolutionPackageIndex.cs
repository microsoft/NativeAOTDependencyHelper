// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.Services;

/// <summary>
/// This is a special root data source which is expected to be used by all other data sources.
/// </summary>
public class SolutionPackageIndex(DotnetToolingInterop _dotnetInterop, ILogger _logger)
{
    public string Name => "Root list of Packages";

    public string Description => "Package information contained within a Solution file retrieves with the dotnet cmdline tool. This is the root source of information for other data sources.";

    public DotnetPackageList? RawPackageList { get; private set; }

    public IEnumerable<NuGetPackageInfo>? Packages { get; private set; }

    public bool HasLoaded { get; private set; }

    public async Task<bool> InitializeAsync(string solutionFilePath)
    {
        HasLoaded = false;
        // https://learn.microsoft.com/dotnet/core/tools/dotnet-list-package
        RawPackageList = await _dotnetInterop.GetTransitiveDependencyListAsync(solutionFilePath);

        if (RawPackageList != null
            && RawPackageList.Projects != null
            && RawPackageList.Projects.Length > 0)
        {
            Packages = NuGetPackageInfo.FromJsonModels(RawPackageList);

            HasLoaded = true;
            _logger.Information($"Loaded Package Information for {solutionFilePath}");
        }
        else
        {
            _logger.Error(new InvalidOperationException("Couldn't load package dependency data"), $"Issue processing solution {solutionFilePath}");
        }

        return HasLoaded;
    }
}

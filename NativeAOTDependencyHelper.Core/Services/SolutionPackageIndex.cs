using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.Services;

/// <summary>
/// This is a special root data source which is expected to be used by all other data sources.
/// </summary>
public class SolutionPackageIndex
{
    public string Name => "Root list of Packages";

    public string Description => "Package information contained within a Solution file retrieves with the dotnet cmdline tool. This is the root source of information for other data sources.";

    public DotnetPackageList? RawPackageList { get; private set; }

    public IEnumerable<NuGetPackageInfo>? Packages { get; private set; }

    public bool HasLoaded { get; private set; }

    public async Task<bool> InitializeAsync(string solutionFilePath)
    {
        // https://learn.microsoft.com/dotnet/core/tools/dotnet-list-package
        RawPackageList = await DotnetToolingInterop.GetTransitiveDependencyListAsync(solutionFilePath);

        if (RawPackageList != null)
        {
            Packages = NuGetPackageInfo.FromJsonModels(RawPackageList);

            HasLoaded = true;
        }

        return HasLoaded;
    }
}

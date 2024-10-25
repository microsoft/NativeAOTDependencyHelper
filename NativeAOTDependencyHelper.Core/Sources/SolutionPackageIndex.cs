using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.Sources;

/// <summary>
/// This is a special root data source which is expected to be used by all other data sources.
/// </summary>
public class SolutionPackageIndex
{
    public string Name => "Root list of Packages";

    public string Description => "Package information contained within a Solution file retrieves with the dotnet cmdline tool. This is the root source of information for other data sources.";

    public Task<bool> InitializeAsync()
    {
        return Task.FromResult(true);
    }
}

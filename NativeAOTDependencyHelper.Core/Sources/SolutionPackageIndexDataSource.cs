using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.Sources;

/// <summary>
/// This is a special root data source which is expected to be used by all other data sources.
/// </summary>
public class SolutionPackageIndexDataSource : IDataSource
{
    public static Guid Id => Guid.Parse("E3A44581-EFD1-416D-A572-53AA52274FB7");

    public static string Name => "Root list of Packages";

    public static string Description => "Package information contained within a Solution file retrieves with the dotnet cmdline tool. This is the root source of information for other data sources.";

    public static Guid[] DependentDataSourceIds => [];

    public Task<bool> InitializeAsync()
    {
        return Task.FromResult(true);
    }
}

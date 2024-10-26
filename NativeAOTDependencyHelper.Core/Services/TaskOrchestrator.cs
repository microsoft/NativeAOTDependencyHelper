using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Sources;

namespace NativeAOTDependencyHelper.Core.Services;

/// <summary>
/// Helper class which orchestrates/reports on all tasks and caches results for reports/checks.
/// </summary>
public class TaskOrchestrator(SolutionPackageIndex _servicePackageIndex)
{
    public async Task<T?> GetDataFromSourceForPackageAsync<T>(IDataSource<T> nugetSource, NuGetPackageInfo package)
    {
        // TODO: We should cache based on the package and only call this once per package...
        return await nugetSource.GetInfoForPackageAsync<T>(package);
    }
}

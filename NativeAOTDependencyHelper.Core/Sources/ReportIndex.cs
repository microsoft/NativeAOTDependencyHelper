using NativeAOTDependencyHelper.Core.Checks;
using NativeAOTDependencyHelper.Core.Models;

namespace NativeAOTDependencyHelper.Core.Sources;

/// <summary>
/// TODO: Ideally this is generated from implementations of our <see cref="IReportItem"/>, <see cref="IAOTCheckItem"/>, and <see cref="IDataSource"/> implementors.
/// </summary>
public static class ReportIndex
{
    public static IDataSource[] Sources => [
        new SolutionPackageIndexDataSource(),
        new NuGetDataSource()
    ];

    /// <summary>
    /// All items here, will sort out in the UI layer/viewmodels?
    /// </summary>
    public static Type[] Items => [
        typeof(NuGetRecentlyUpdated),
    ];
}

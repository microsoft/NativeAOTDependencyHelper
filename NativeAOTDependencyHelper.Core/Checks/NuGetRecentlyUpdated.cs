using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Sources;

namespace NativeAOTDependencyHelper.Core.Checks;

public class NuGetRecentlyUpdated : IAOTCheckItem
{
    public static string Name => "Nuget Package Up-to-date";

    public static int SortOrder => 5;

    public static Guid[] DependentDataSourceIds => [NuGetDataSource.Id];

    public string ReportDetails { get; } = string.Empty;

    public Uri? ResultDetailLink { get; }

    public bool HasPassed { get; }

    public static IReportItem Process(IDataSource[] sources)
    {
        if (sources != null
            && sources.Length > 0
            && sources[0] is NuGetDataSource nuget)
        {
        }

        throw new NotImplementedException();
    }
}

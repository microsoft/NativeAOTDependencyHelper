namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines an item to report general data back from a <see cref="IDataSource"/>.
/// </summary>
public interface IReportItem : IDependsOnDataSource
{
    /// <summary>
    /// Gets the name of the report item.
    /// </summary>
    public static abstract string Name { get; }

    /// <summary>
    /// Gets the desired sort order for this item within the list of reports.
    /// </summary>
    public static abstract int SortOrder { get; }

    /// <summary>
    /// Gets additional detail text about the state of the report.
    /// </summary>
    public string ReportDetails { get; }

    /// <summary>
    /// Gets (optionally) a link to direct to the result/source of the report information.
    /// </summary>
    public Uri? ResultDetailLink { get; }

    /// <summary>
    /// Processes data from the given data sources (will happen for each package) and returns a new instance of <see cref="IReportItem"/> to add to that package's checklist.
    /// </summary>
    /// <param name="sources">Instances of the requested <see cref="IDataSource"/> in <see cref="IDependsOnDataSource.DependentDataSourceIds"/>, will be in the same order as requested or empty if none.</param>
    /// <returns>A new <see cref="IReportItem"/> for the given package source</returns>
    public static abstract IReportItem Process(IDataSource[] sources);
}

namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines a singleton provider of <see cref="ReportItem"/> items.
/// Use constructor injection to get access to desired <see cref="IDataSource"/>.
/// </summary>
public interface IReportItemProvider
{
    /// <summary>
    /// Gets the name of the report item provider.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the <see cref="ReportCategory"/> category of the information this provider provides.
    /// </summary>
    public ReportCategory Category { get; }

    /// <summary>
    /// Gets the desired sort order for this item within the list of reports.
    /// </summary>
    public int SortOrder { get; }

    /// <summary>
    /// Gets a description of the report type for the provider.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Processes data from the given data sources (will happen for each package) and returns a new instance of <see cref="IReportItemProvider"/> to add to that package's checklist.
    /// </summary>
    /// <param name="sources">Instances of the requested <see cref="IDataSource"/> in <see cref="IDependsOnDataSource.DependentDataSourceIds"/>, will be in the same order as requested or empty if none.</param>
    /// <returns>A new <see cref="ReportItem"/> (or <see cref="AOTCheckItem"/>) for the given <see cref="NuGetPackageInfo"/>.</returns>
    public Task<ReportItem> ProcessPackage(NuGetPackageInfo package);
}

public enum ReportCategory
{
    Informational,
    Health,
    AOTCompatibility
}
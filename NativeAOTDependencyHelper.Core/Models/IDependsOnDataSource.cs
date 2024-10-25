namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Interface which is used by <see cref="IAOTCheckItem"/>, <see cref="IReportItem"/>, and other <see cref="IDataSource"/> to declare which other data sources they depend on to process for information.
/// 
/// The check/report/datasource will only get processed once all dependencies have finished their step for a given package.
/// </summary>
public interface IDependsOnDataSource
{
    /// <summary>
    /// Gets the list of dependent <see cref="IDataSource"/> required before being able to process this step.
    /// If empty, step will be run right away on available package info.
    /// </summary>
    public static abstract Guid[] DependentDataSourceIds { get; }
}

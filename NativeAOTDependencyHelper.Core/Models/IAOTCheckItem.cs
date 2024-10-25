namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines a special type of <see cref="IReportItem"/> that additional performs a validation check that'll report status towards AOT compatibility reliant on an <see cref="IDataSource"/>.
/// </summary>
public interface IAOTCheckItem : IReportItem, IDependsOnDataSource
{
    /// <summary>
    /// Gets if the check was passed successfully or not.
    /// </summary>
    public bool HasPassed { get; }
}

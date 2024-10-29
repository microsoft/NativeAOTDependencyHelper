namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines a source of data which will be needed for various <see cref="ReportItem"/> or <see cref="AOTCheckItem"/> (or even other <see cref="IDataSource"/>).
/// </summary>
public interface IDataSource
{
    public string Name { get; }

    public string Description { get; }

    /// <summary>
    /// Gets whether or not this data source has been initialized and is ready for requests via <see cref="GetInfoForPackageAsync{T}(NuGetPackageInfo)"/>. If this is false, the <see cref="NativeAOTDependencyHelper.Core.Services.TaskOrchestrator"/> will call <see cref="InitializeAsync"/>. Be sure to set this to <c>true</c> once your <see cref="InitializeAsync"/> has been called.
    /// </summary>
    public bool IsInitialized { get; }

    // TODO: Probably need a flag for if we're in an error state and can't continue?

    /// <summary>
    /// Called before the data source is used in case anything needs to be setup ahead of time.
    /// </summary>
    /// <returns>True if the datasource is ready to retrieve more data about packages.</returns>
    public Task<bool> InitializeAsync();

    /// <summary>
    /// Called by the <see cref="NativeAOTDependencyHelper.Core.Services.TaskOrchestrator"/> to retrieve the requested metadata for the given package. This result will be cached for other <see cref="IReportItemProvider"/>s to use if looking at the same info for this data source.
    /// </summary>
    /// <typeparam name="T">The metadata type this datasource returns.</typeparam>
    /// <param name="package">The <see cref="NuGetPackageInfo"/> for the package that data is being requested.</param>
    /// <returns>The requested information of the requested type parameter from the data source or null.</returns>
    public Task<T?> GetInfoForPackageAsync<T>(NuGetPackageInfo package);
}

/*
 * dotnet data [Main Source]
NuGet.org data [Source]
  recently updated package [Check]
  are you using latest package? [Check]
  Get repository Info [Source]
    Is code available? [Report]
    Get GitHub Data [Source]
      recently committed [Check]
      code contains "<IsAotCompatible>" near "true" [Check]
    Get GitHub Issues [Source]
      Are there open issues with "AOT" [Check]
      Number/Links to issues containing "AOT" [Report]
*/
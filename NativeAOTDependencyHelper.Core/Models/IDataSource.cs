namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines a source of data which will be needed for various <see cref="AOTCheckItem"/>
/// </summary>
public interface IDataSource<T> // TODO: Needed to make this generic for how we implement the InfoForPackage call, but then not sure how this will effect getting an aggregate list from the service provider... maybe that doesn't matter?
{
    public string Name { get; }

    public string Description { get; }    

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
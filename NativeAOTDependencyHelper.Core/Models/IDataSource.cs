namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines a source of data which will be needed for various <see cref="AOTCheckItem"/>
/// </summary>
public interface IDataSource
{
    public string Name { get; }

    public string Description { get; }

    /// <summary>
    /// Called before the data source is used in case anything needs to be setup ahead of time.
    /// </summary>
    /// <returns>True if the datasource is ready to retrieve more data about packages.</returns>
    public Task<bool> InitializeAsync();
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
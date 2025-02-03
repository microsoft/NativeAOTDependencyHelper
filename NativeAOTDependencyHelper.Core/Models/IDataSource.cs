// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines a source of data which will be needed for various <see cref="AOTCheckItem"/>
/// </summary>
public interface IDataSource<T> // TODO: Needed to make this generic for how we implement the InfoForPackage call, but then not sure how this will effect getting an aggregate list from the service provider... maybe that doesn't matter?
{
    public string Name { get; }

    public string Description { get; }

    /// <summary>
    /// Gets whether or not this data source has been initialized and is ready for requests via <see cref="GetInfoForPackageAsync(NuGetPackageInfo)"/>. If this is false, the <see cref="NativeAOTDependencyHelper.Core.Services.TaskOrchestrator"/> will call <see cref="InitializeAsync"/>. Be sure to set this to <c>true</c> once your <see cref="InitializeAsync"/> has been called.
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
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The requested information of the requested type parameter from the data source or null.</returns>
    public Task<T?> GetInfoForPackageAsync(NuGetPackageInfo package, CancellationToken cancellationToken);
}

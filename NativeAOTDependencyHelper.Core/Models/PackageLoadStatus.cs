namespace NativeAOTDependencyHelper.Core.Models;

public enum PackageLoadStatus
{
    /// <summary>
    /// Package was successfully loaded and processed.
    /// </summary>
    Success,
    /// <summary>
    /// Package data is currently loading
    /// </summary>
    Loading,
    /// <summary>
    /// Package was found but there was an error processing it.
    /// </summary>
    Error
}
namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Represents a .NET Project's reference to a NuGet package, contained within <see cref="NuGetPackageInfo"/>.
/// </summary>
/// <param name="ParentProjectPath">The filepath to the project that uses this package.</param>
/// <param name="Framework">The target framework this package is evaluating for.</param>
/// <param name="RequestedVersion">The version which the project requested for this package.</param>
/// <param name="ResolvedVersion">The version NuGet decided to use for this package.</param>
/// <param name="TransitiveLayer">How many levels removed is this package from the top-level packages for this project? If <c>zero</c>, it is a top-level reference directly included by a project.</param>
public record NuGetPackageProjectReference(string ParentProjectPath,
                                           string Framework,
                                           string RequestedVersion,
                                           string ResolvedVersion,
                                           int TransitiveLayer)
{
    /// <summary>
    /// Gets whether this is a transitive reference or not (dependency of another directly referenced package). <c>true</c> when <see cref="TransitiveLayer"/> is non-zero.
    /// </summary>
    public bool IsTransitive => TransitiveLayer > 0;

    public string Name => Path.GetFileName(ParentProjectPath);
}

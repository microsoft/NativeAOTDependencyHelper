using NativeAOTDependencyHelper.Core.JsonModels;

namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Core base class representing base-level nuget package info from solution file.
/// Remapped from the Json data provided by <see cref="DotnetPackageList"/>.
/// </summary>
/// <param name="ParentProjectPath">The filepath to the project that uses this package.</param>
/// <param name="Name">The <see cref="NuGetPackage.Id"/> of this package.</param>
/// <param name="Framework">The target framework this package is evaluating for.</param>
/// <param name="RequestedVersion">The version which the project requested for this package.</param>
/// <param name="ResolvedVersion">The version NuGet decided to use for this package.</param>
public record NuGetPackageInfo(string ParentProjectPath,
                               string Name,
                               string Framework,
                               string RequestedVersion,
                               string ResolvedVersion)
{
    /// <summary>
    /// Gets if this package is a transitive reference and required for a referenced package. If <c>false</c>, then the package explicitly requested to use this package.
    /// </summary>
    public bool IsTransitive { get; init; }

    /// <summary>
    /// Takes the Json data from <see cref="DotnetPackageList"/> and flattens into a straight list of <see cref="NuGetPackageInfo"/> for passing to <see cref="IReportItemProvider"/>.
    /// </summary>
    /// <param name="packageList"></param>
    /// <returns></returns>
    public static IEnumerable<NuGetPackageInfo> FromJsonModels(DotnetPackageList packageList)
    {
        // TODO: Would be nice if we can stream the Json deserialization and then process things by package one-by-one here too?
        int commonPathIndex = GetFirstDifferentCharacter(packageList.Projects.Select(p => p.Path));

        if (commonPathIndex < 0)
        {
            throw new IOException("Invalid project path. Please use a valid .sln file");
        }

        foreach (var project in packageList.Projects)
        {
            foreach (var framework in project.Frameworks)
            {
                foreach (var package in framework.TopLevelPackages)
                {
                    yield return new NuGetPackageInfo(
                        ParentProjectPath: project.Path.Substring(commonPathIndex),
                        Name: package.Id,
                        Framework: framework.Framework,
                        RequestedVersion: package.RequestedVersion,
                        ResolvedVersion: package.ResolvedVersion);
                }

                foreach (var package in framework.TransitivePackages)
                {
                    yield return new NuGetPackageInfo(
                        ParentProjectPath: project.Path.Substring(commonPathIndex),
                        Name: package.Id,
                        Framework: framework.Framework,
                        RequestedVersion: package.RequestedVersion,
                        ResolvedVersion: package.ResolvedVersion)
                    {
                        IsTransitive = true
                    };
                }
            }
        }
    }

    /// <summary>
    /// Given a list of strings, finds the first index where there's not the same character at that index.
    /// </summary>
    /// <param name="enumerable">List of strings</param>
    /// <returns>index of first difference, -1 if null or only single string</returns>
    private static int GetFirstDifferentCharacter(IEnumerable<string> strings)
    {
        if (strings == null) return -1;
        if (strings.Count() == 1) return 0;

        int i = 0;
        string firstString = strings.First();

        while (firstString.Length > i)
        {
            foreach (var str in strings.Skip(1))
            {
                // If any of the other strings characters don't match our firststring character, that's it
                if (str.Length == i || firstString[i] != str[i])
                {
                    return i;
                }
            }
            i++;
        }

        return i;
    }
}

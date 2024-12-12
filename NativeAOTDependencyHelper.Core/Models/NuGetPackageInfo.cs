using Microsoft.Build.Evaluation;
using NativeAOTDependencyHelper.Core.JsonModels;

namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Core base class representing base-level nuget package info from solution file.
/// Remapped from the Json data provided by <see cref="DotnetPackageList"/>.
/// </summary>
/// <param name="Name">The <see cref="NuGetPackage.Id"/> of this package.</param>
/// <param name="ProjectReferences">List of projects which reference this package.</param> // TODO: This could be a problem being a record-type if we add additional references here by transitive dependencies...
public record NuGetPackageInfo(string Name, NuGetPackageProjectReference[] ProjectReferences)
{
    /// <summary>
    /// Gets whether this NuGetPackage is ONLY included due to transitive references and has no direct references by projects.
    /// </summary>
    public bool IsTransitiveOnly => ProjectReferences.All(p => p.IsTransitive);

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

        Dictionary<string, List<NuGetPackageProjectReference>> _uniquePackageIndex = new();

        foreach (var project in packageList.Projects)
        {
            // If no frameworks, it is probably because the project was not restored yet.
            // In that case, we need to load the project file and get the package references directly.
            if (project.Frameworks.Count() == 0)
            {
                var projectCollection = new ProjectCollection();
                var msBuildProject = projectCollection.LoadProject(project.Path);

                // Get all direct package references
                var packageRefs = msBuildProject.GetItems("PackageReference");

                foreach (var p in packageRefs)
                {
                    string packageName = p.EvaluatedInclude;
                    string version = p.GetMetadataValue("Version");
                    if (!_uniquePackageIndex.ContainsKey(packageName))
                    {
                        _uniquePackageIndex[packageName] = [];
                    }
                    _uniquePackageIndex[packageName].Add(new NuGetPackageProjectReference(
                        ParentProjectPath: project.Path.Substring(commonPathIndex),
                        Framework: string.Empty,
                        RequestedVersion: version,
                        ResolvedVersion: version,
                        TransitiveLayer: 0));
                }
            }
            else
            {
                foreach (var framework in project.Frameworks)
                {
                    // Layer is either TopLevel (0) or Transitive (1)
                    for (int layer = 0; layer <= 1; layer++)
                    {
                        // See Switch Expression: https://learn.microsoft.com/dotnet/csharp/language-reference/operators/switch-expression
                        foreach (var package in layer switch { 0 => framework.TopLevelPackages,
                                                               1 => framework.TransitivePackages,
                                                               _ => throw new IndexOutOfRangeException() })
                        {
                            if (!_uniquePackageIndex.ContainsKey(package.Id))
                            {
                                _uniquePackageIndex[package.Id] = new();
                            }

                            _uniquePackageIndex[package.Id].Add(new NuGetPackageProjectReference(
                                ParentProjectPath: project.Path.Substring(commonPathIndex),
                                Framework: framework.Framework,
                                RequestedVersion: package.RequestedVersion,
                                ResolvedVersion: package.ResolvedVersion,
                                TransitiveLayer: layer));
                        }
                    }
                }
            }
        }

        // Construct final package info with references to all projects used
        foreach ((var packageId, var projects) in _uniquePackageIndex)
        {
            yield return new NuGetPackageInfo(packageId, projects.ToArray());
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

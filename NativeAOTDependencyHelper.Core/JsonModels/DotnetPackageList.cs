using System.Text.Json.Serialization;

namespace NativeAOTDependencyHelper.Core.JsonModels;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class DotnetPackageList
{
    public int Version { get; set; }

    public string Parameters { get; set; } = string.Empty;

    public Project[] Projects { get; set; } = Array.Empty<Project>();
}

public class Project
{
    public string Path { get; set; } = string.Empty;

    public ProjectFramework[] Frameworks { get; set; } = Array.Empty<ProjectFramework>();
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class ProjectFramework
{
    public string Framework { get; set; } = string.Empty;

    public NuGetPackage[] TopLevelPackages { get; set; } = Array.Empty<NuGetPackage>();

    public NuGetPackage[] TransitivePackages { get; set; } = Array.Empty<NuGetPackage>();
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetPackage
{
    public string Id { get; set; } = string.Empty;

    public string RequestedVersion { get; set; } = string.Empty;

    public string ResolvedVersion { get; set; } = string.Empty;
}

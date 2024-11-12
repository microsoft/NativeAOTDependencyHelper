using System.Text.Json.Serialization;

namespace NativeAOTDependencyHelper.Core.JsonModels;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetPackageRegistration
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@type")]
    public string[] Type { get; set; } = Array.Empty<string>();
    public string CommitId { get; set; } = string.Empty;
    public DateTime CommitTimeStamp { get; set; } = DateTime.MinValue;
    public int Count { get; set; } = 0;
    public RegistrationListings[] Items { get; set; } = Array.Empty<RegistrationListings>();
    public NuGetRegistrationContext? Context { get; set; }
    public string? RepositoryUrl { get; set; }

    public string? Error { get; set; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetRegistrationContext
{
    public string Vocab { get; set; } = string.Empty;
    public string Catalog { get; set; } = string.Empty;
    public string Xsd { get; set; } = string.Empty;
    public Items Items { get; set; } = new Items();
    public CommitTimeStamp CommitTimeStamp { get; set; } = new CommitTimeStamp();
    public CommitId CommitId { get; set; } = new CommitId();
    public Count Count { get; set; } = new Count();
    public Parent Parent { get; set; } = new Parent();
    public Tags Tags { get; set; } = new Tags();
    public Reasons Reasons { get; set; } = new Reasons();
    public PackageTargetFrameworks PackageTargetFrameworks { get; set; } = new PackageTargetFrameworks();
    public DependencyGroups DependencyGroups { get; set; } = new DependencyGroups();
    public Dependencies Dependencies { get; set; } = new Dependencies();
    public PackageContent PackageContent { get; set; } = new PackageContent();
    public Published Published { get; set; } = new Published();
    public Registration Registration { get; set; } = new Registration();
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Items
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@container")]
    public string Container { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class CommitTimeStamp
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class CommitId
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Count
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Parent
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Tags
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@container")]
    public string Container { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Reasons
{
    [JsonPropertyName("@container")]
    public string Container { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class PackageTargetFrameworks
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("@container")]
    public string Container { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class DependencyGroups
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@container")]
    public string Container { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Dependencies
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("@container")]
    public string Container { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class PackageContent
{
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Published
{
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class Registration
{
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class RegistrationListings
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    public string CommitId { get; set; } = string.Empty;
    public DateTime CommitTimeStamp { get; set; } = DateTime.MinValue;
    public int Count { get; set; } = 0;
    public CatalogEntryMetadata[] Items { get; set; } = Array.Empty<CatalogEntryMetadata>();
    public string Parent { get; set; } = string.Empty;
    public string Lower { get; set; } = string.Empty;
    public string Upper { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class CatalogEntryMetadata
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    public string CommitId { get; set; } = string.Empty;
    public DateTime CommitTimeStamp { get; set; } = DateTime.MinValue;
    public CatalogEntry CatalogEntry { get; set; } = new CatalogEntry();
    public string PackageContent { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class CatalogEntry
{
    [JsonPropertyName("@id")]
    public string IdUrl { get; set; } = string.Empty;
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public PackageDependencyGroup[] DependencyGroups { get; set; } = Array.Empty<PackageDependencyGroup>();
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    [JsonPropertyName("id")]
    public string IdName { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string LicenseExpression { get; set; } = string.Empty;
    public string LicenseUrl { get; set; } = string.Empty;
    public bool Listed { get; set; } = false;
    public string MinClientVersion { get; set; } = string.Empty;
    public string PackageContent { get; set; } = string.Empty;
    public string ProjectUrl { get; set; } = string.Empty;
    public DateTime Published { get; set; } = DateTime.MinValue;
    public bool RequireLicenseAcceptance { get; set; } = false;
    public string Summary { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class PackageDependencyGroup
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;
    public string TargetFramework { get; set; } = string.Empty;
    public PackageDependency[] Dependencies { get; set; } = Array.Empty<PackageDependency>();
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class PackageDependency
{
    [JsonPropertyName("@id")]
    public string IdUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string IdName { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
}

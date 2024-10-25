using System.Text.Json.Serialization;

namespace NativeAOTDependencyHelper.Core.JsonModels;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetServiceIndex
{
    public string Version { get; set; } = string.Empty;

    public NuGetResource[] Resources { get; set; } = Array.Empty<NuGetResource>();

    public NuGetServiceContext? Context { get; set; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetServiceContext
{
    public string Vocab { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetResource
{
    public string Id { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public string ClientVersion { get; set; } = string.Empty;
}

// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace NativeAOTDependencyHelper.Core.JsonModels;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetServiceIndex
{
    public string Version { get; set; } = string.Empty;
    public NuGetResource[] Resources { get; set; } = Array.Empty<NuGetResource>();

    [JsonPropertyName("@context")]
    public NuGetServiceContext? Context { get; set; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetServiceContext
{
    [JsonPropertyName("@vocab")]
    public string Vocab { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class NuGetResource
{
    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("@type")]
    public string Type { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public string ClientVersion { get; set; } = string.Empty;
}

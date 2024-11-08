using System.Text.Json.Serialization;

namespace NativeAOTDependencyHelper.Core.JsonModels;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class GitHubCodeSearchResult
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Encoding { get; set; } = string.Empty;

    public bool? IsAotCompatible { get; set; }
}

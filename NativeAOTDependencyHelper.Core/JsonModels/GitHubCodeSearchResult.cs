// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.Models;
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

    public CheckStatus CheckStatus { get; set; } = CheckStatus.Unavailable;

    public string? Error { get; set; }
}

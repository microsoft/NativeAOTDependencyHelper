// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Services;
using System.Diagnostics;
using System.Text.Json;

namespace NativeAOTDependencyHelper.Core;

public class DotnetToolingInterop(ILogger _logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<string> CheckDotnetVersion()
    {
        Process process = new();
        process.StartInfo.FileName = "dotnet.exe";
        process.StartInfo.Arguments = "--version";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        var version = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return version;
    }

    public async Task<DotnetPackageList?> GetTransitiveDependencyListAsync(string solutionpath)
    {
        try
        {
            Process process = new();
            process.StartInfo.FileName = "dotnet.exe";
            process.StartInfo.Arguments = $"list {solutionpath} package --include-transitive --format json";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            var json = await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (json.StartsWith("error:"))
            {
                _logger.Error(new InvalidOperationException(json), $"Error processing solution file {solutionpath}");
                return null;
            }

            return JsonSerializer.Deserialize<DotnetPackageList>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Issue parsing output of `dotnet list {solutionpath} package --include-transitive --format json`");
            return null;
        }
    }
}

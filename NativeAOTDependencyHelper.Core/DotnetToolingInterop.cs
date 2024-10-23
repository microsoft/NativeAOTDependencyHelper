using NativeAOTDependencyHelper.Core.JsonModels;
using System.Diagnostics;
using System.Text.Json;

namespace NativeAOTDependencyHelper.Core;

public class DotnetToolingInterop
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task<string> CheckDotnetVersion()
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

    public static async Task<DotnetPackageList?> GetTransitiveDependencyListAsync(string solutionpath)
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

        return JsonSerializer.Deserialize<DotnetPackageList>(json, JsonOptions);
    }
}

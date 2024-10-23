using System.Diagnostics;

namespace NativeAOTDependencyHelper.Core;

public class DotnetToolingInterop
{
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
}

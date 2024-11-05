using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;
using System.Text;

namespace NativeAOTDependencyHelper.Core.Checks;
public class GitHubAotCheck(TaskOrchestrator _orchestrator, IDataSource<GitHubCodeSearchResult> _gitHubSource) : IReportItemProvider
{
    public string Name => "IsAotCompatible Flag";

    public int SortOrder => 10;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<GitHubCodeSearchResult>(_gitHubSource, package);
        if (packageMetadata == null) return new AOTCheckItem(this, false, "Flag not found for package.");
        return new AOTCheckItem(this, (packageMetadata.IsAotCompatible == true), $"Source code URL: {packageMetadata.DownloadUrl}");
    }
}

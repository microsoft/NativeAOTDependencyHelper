using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Checks;

public class GitHubAotFlagCheck(TaskOrchestrator _orchestrator, IDataSource<GitHubCodeSearchResult> _gitHubSource) : IReportItemProvider
{
    public string Name => "IsAotCompatible Flag";

    public int SortOrder => 10;

    public ReportCategory Category => ReportCategory.AOTCompatibility;

    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<GitHubCodeSearchResult>(_gitHubSource, package);
        if (packageMetadata == null) return new AOTCheckItem(this, CheckStatus.Unavailable, "Flag not found for package.");
        if (packageMetadata.DownloadUrl == null) return new AOTCheckItem(this, packageMetadata.CheckStatus, "Flag found, but source file could not be retrieved. Please check repository for more details.");
        return new AOTCheckItem(this, packageMetadata.CheckStatus, "Click to navigate to source file", new Uri(packageMetadata.DownloadUrl));
    }
}

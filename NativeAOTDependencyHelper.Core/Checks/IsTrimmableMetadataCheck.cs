using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;


namespace NativeAOTDependencyHelper.Core.Checks;
public class IsTrimmableMetadataCheck(TaskOrchestrator _orchestrator, IDataSource<bool> _nugetSource) : IReportItemProvider
{
    public string Name => "IsTrimmable Assembly Flag";
    public ReportCategory Category => ReportCategory.AOTCompatibility;
    public int SortOrder => 4;
    public string Description => "Is the package assembly metadata marked as trimmable?";
    public ReportType Type => ReportType.Check;
    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        var isTrimmable = await _orchestrator.GetDataFromSourceForPackageAsync<bool>(_nugetSource, package, cancellationToken);
        return isTrimmable 
            ? new AOTCheckItem(this, CheckStatus.Passed, "Assembly is marked as trimmable") 
            : new AOTCheckItem(this, CheckStatus.Warning, "Assembly is not marked as trimmable. However, if IsAotCompatible flag is true, then the package is also trimmable.");
    }

}

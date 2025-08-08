// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;


namespace NativeAOTDependencyHelper.Core.Checks;
public class IsTrimmableMetadataCheck(TaskOrchestrator _orchestrator, IDataSource<NuGetPackageRegistration> _nugetSource) : IReportItemProvider
{
    public string Name => "IsTrimmable Assembly Flag";
    public ReportCategory Category => ReportCategory.AOTCompatibility;
    public int SortOrder => 4;
    public string Description => "Is the package assembly metadata marked as trimmable?";
    public ReportType Type => ReportType.Check;
    public async Task<ReportItem> ProcessPackage(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<NuGetPackageRegistration>(_nugetSource, package, cancellationToken);
        if (packageMetadata != null)
        {
            if (packageMetadata.IsTrimmable) return new AOTCheckItem(this, CheckStatus.Passed, "Assembly is marked as trimmable");
            return new AOTCheckItem(this, CheckStatus.Warning, "Assembly is not marked as trimmable. However, if IsAotCompatible flag is true, then the package is also trimmable.");
        }
        return new AOTCheckItem(this, CheckStatus.Error, "Error fetching NuGet package registration.");
    }

}

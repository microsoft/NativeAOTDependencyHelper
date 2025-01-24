// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core.Checks;

public class GitHubAotFlagCheck(TaskOrchestrator _orchestrator, IDataSource<GitHubCodeSearchResult> _gitHubSource) : IReportItemProvider
{
    public string Name => "IsAotCompatible Flag";

    public int SortOrder => 10;

    public ReportCategory Category => ReportCategory.AOTCompatibility;

    public ReportType Type => ReportType.Check;

    public string Description => "Searches GitHub source code (if available) for the <IsAotCompatible> project flag";

    public async Task<ReportItem?> ProcessPackage(NuGetPackageInfo package, CancellationToken cancellationToken)
    {
        var packageMetadata = await _orchestrator.GetDataFromSourceForPackageAsync<GitHubCodeSearchResult>(_gitHubSource, package, cancellationToken);
        if (packageMetadata == null) return new AOTCheckItem(this, CheckStatus.Unavailable, "Flag not found for package.");
        if (packageMetadata.Error != null) return new AOTCheckItem(this, CheckStatus.Error, $"Error performing GitHub code search: {packageMetadata.Error}", null, "Error performing GitHub AOT tag code search.");
        if (packageMetadata.DownloadUrl == null) return new AOTCheckItem(this, packageMetadata.CheckStatus, "Flag found, but source file could not be retrieved. Please check repository for more details.");
        return new AOTCheckItem(this, packageMetadata.CheckStatus, "Click to navigate to source file", new Uri(packageMetadata.DownloadUrl));
    }
}

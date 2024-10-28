namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines an item result to report general data back from a <see cref="IDataSource"/>.
/// </summary>
/// <param name="Provider">reference to the provider of this data</param>
/// <param name="ReportDetails">additional detail text about the state of the report</param>
/// <param name="ResultDetailLink">(optionally) a link to direct to the result/source of the report information.</param>
public record ReportItem(IReportItemProvider Provider, string ReportDetails, Uri? ResultDetailLink = null);

/// <summary>
/// Defines a special type of <see cref="ReportItem"/> that additional performs a validation check, <see cref="HasPassed"/>; that'll report status towards AOT compatibility reliant on an <see cref="IDataSource"/>.
/// </summary>
/// <param name="HasPassed">if the check was passed successfully or not</param>
/// <param name="Provider">reference to the provider of this data</param>
/// <param name="ReportDetails">additional detail text about the state of the report</param>
/// <param name="ResultDetailLink">(optionally) a link to direct to the result/source of the report information.</param>
public record AOTCheckItem(IReportItemProvider Provider, bool HasPassed, string ReportDetails, Uri? ResultDetailLink = null) : ReportItem(Provider, ReportDetails, ResultDetailLink);

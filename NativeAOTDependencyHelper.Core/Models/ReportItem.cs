// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace NativeAOTDependencyHelper.Core.Models;

/// <summary>
/// Defines an item result to report general data back from a <see cref="IDataSource"/>.
/// </summary>
/// <param name="Provider">reference to the provider of this data</param>
/// <param name="ReportDetails">additional detail text about the state of the report</param>
/// <param name="ResultDetailLink">(optionally) a link to direct to the result/source of the report information.</param>
public record ReportItem(IReportItemProvider Provider, string ReportDetails, Uri? ResultDetailLink = null, string? ProcessingError = null);

/// <summary>
/// Defines a special type of <see cref="ReportItem"/> that additional performs a validation check, <see cref="Status"/>; that'll report status towards AOT compatibility reliant on an <see cref="IDataSource"/>.
/// </summary>
/// <param name="Status">the status/result of the check, <see cref="CheckStatus"/></param>
/// <param name="Provider">reference to the provider of this data</param>
/// <param name="ReportDetails">additional detail text about the state of the report</param>
/// <param name="ResultDetailLink">(optionally) a link to direct to the result/source of the report information.</param>
public record AOTCheckItem(IReportItemProvider Provider, CheckStatus Status, string ReportDetails, Uri? ResultDetailLink = null, string? ProcessingError = null) : ReportItem(Provider, ReportDetails, ResultDetailLink, ProcessingError);

/// <summary>
/// The various possible outcomes of a check returnable in an <see cref="AOTCheckItem"/>.
/// </summary>
public enum CheckStatus
{
    /// <summary>
    /// Returned if the check encountered an error while processing.
    /// </summary>
    Error,
    /// <summary>
    /// Returned when the check has been performed, but the result requires investigation/indicates a potential issue with AOT compatibility.
    /// </summary>
    Warning,
    /// <summary>
    /// Returned when the check has passed and does not indicate a potential issue with AOT compatibility; Note: this does not guarantee AOT compatibility.
    /// </summary>
    Passed,
    /// <summary>
    /// Returned when the source to perform the check is unavailable and the check cannot be verified
    /// </summary>
    Unavailable
}
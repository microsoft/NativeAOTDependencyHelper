// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace NativeAOTDependencyHelper.Core.Models;

public enum PackageLoadStatus
{
    /// <summary>
    /// Package was successfully loaded and processed.
    /// </summary>
    Success,
    /// <summary>
    /// Package data is currently loading
    /// </summary>
    Loading,
    /// <summary>
    /// Package was found but there was an error processing it.
    /// </summary>
    Error
}
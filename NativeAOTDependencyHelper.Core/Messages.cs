// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core;

/// <summary>
/// Message sent when an error has been logged in the <see cref="ILogger"/>.
/// </summary>
public record LoggedErrorMessage(string Message);
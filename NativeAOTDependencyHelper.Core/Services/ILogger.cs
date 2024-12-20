// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace NativeAOTDependencyHelper.Core.Services;

public interface ILogger
{
    void Information(string message, [CallerMemberName] string member = "");

    void Warning(string message, [CallerMemberName] string member = "");

    void Error(Exception exception, string message, [CallerMemberName] string member = "");
}

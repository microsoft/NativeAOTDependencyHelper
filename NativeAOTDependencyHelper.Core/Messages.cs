using NativeAOTDependencyHelper.Core.Services;

namespace NativeAOTDependencyHelper.Core;

/// <summary>
/// Message sent when an error has been logged in the <see cref="ILogger"/>.
/// </summary>
public record LoggedErrorMessage(string Message);
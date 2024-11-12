
namespace NativeAOTDependencyHelper.ViewModels.Services;

public record LogItemData(string Message, LogItemLevel Level)
{
}

public enum LogItemLevel
{
    Info,
    Warning,
    Error,
}


using System.Runtime.CompilerServices;

namespace NativeAOTDependencyHelper.Core.Services;

public interface ILogger
{
    void Information(string message, [CallerMemberName] string member = "");

    void Warning(string message, [CallerMemberName] string member = "");

    void Error(Exception exception, string message, [CallerMemberName] string member = "");
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using NativeAOTDependencyHelper.Core.Checks;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Sources;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NativeAOTDependencyHelper.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window _window;

    public nint MainWindowHwnd => WinRT.Interop.WindowNative.GetWindowHandle(_window);

    public IServiceProvider Services { get; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Services = ConfigureServices();

        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection services = new();

        // Root service
        services.AddSingleton<SolutionPackageIndex>();
        
        // Data Sources
        services.AddSingleton<IDataSource, NuGetDataSource>();

        // Reports/Checks
        services.AddSingleton<IReportItemProvider, NuGetRecentlyUpdated>();

        return services.BuildServiceProvider();
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using NativeAOTDependencyHelper.Core.Checks;
using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Reports;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.Core.Sources;
using NativeAOTDependencyHelper.ViewModels;
using NativeAOTDependencyHelper.ViewModels.Services;
using System;
using System.Threading.Tasks;

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

        // View Models
        services.AddSingleton<MainViewModel>();

        // Root service
        services.AddSingleton<SolutionPackageIndex>();
        services.AddSingleton<TaskOrchestrator>();
        services.AddSingleton(TaskScheduler.FromCurrentSynchronizationContext()); // Grab copy of UI thread
        
        // Data Sources
        services.AddSingleton<IDataSource<NuGetPackageRegistration>, NuGetDataSource>();
        services.AddSingleton<IDataSource<GitHubIssueSearchResult>, GitHubIssueSearchDataSource>();
        services.AddSingleton<IDataSource<GitHubCodeSearchResult>, GitHubAotCompatibleCodeSearchDataSource>();

        // Other services
        services.AddSingleton<ILogger, BasicLogger>();
        services.AddSingleton<GitHubOAuthService>();

        // Checks
        services.AddSingleton<IReportItemProvider, NuGetLatestVersionCheck>();
        services.AddSingleton<IReportItemProvider, NuGetRecentlyUpdatedCheck>();
        services.AddSingleton<IReportItemProvider, GitHubAotFlagCheck>();
        services.AddSingleton<IReportItemProvider, GitHubAotIssuesCheck>();

        // Reports
        services.AddSingleton<IReportItemProvider, NuGetLicenseReport>();

        return services.BuildServiceProvider();
    }
}

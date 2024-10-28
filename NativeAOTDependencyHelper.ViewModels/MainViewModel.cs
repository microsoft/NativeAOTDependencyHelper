using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Models;
using NativeAOTDependencyHelper.Core.Sources;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NativeAOTDependencyHelper.ViewModels;

public partial class MainViewModel(IServiceProvider _serviceProvider) : ObservableObject
{
    public IAsyncRelayCommand DotnetVersionCommand { get; } = new AsyncRelayCommand(DotnetToolingInterop.CheckDotnetVersion);

    [ObservableProperty]
    private ObservableCollection<NuGetPackageViewModel> _packages = new();

    // TODO: Have error string to report back issues initializing?

    [RelayCommand]
    public async Task ProcessSolutionFileAsync(string filepath)
    {
        // TODO: Move this and Packages creation to the SolutionPackageIndex...
        // https://learn.microsoft.com/dotnet/core/tools/dotnet-list-package
        var dotnetPackageList = await DotnetToolingInterop.GetTransitiveDependencyListAsync(filepath);
        var nugetDataSource = new NuGetDataSource();
        await nugetDataSource.InitializeAsync();

        if (dotnetPackageList != null)
        {
            Packages = new(NuGetPackageInfo.FromJsonModels(dotnetPackageList).Select(p => new NuGetPackageViewModel(p)));

            // TODO: Move this to the orchestrator (though need to figure out connection of report item processing back in NuGetPackageViewModel
            foreach (var package in Packages)
            {
                foreach (var reporter in _serviceProvider.GetServices<IReportItemProvider>())
                {
                    var report = await reporter.ProcessPackage(package.Info);
                    if (report is AOTCheckItem check)
                    {
                        package.CheckItems.Add(check);
                    }
                    else if (report is ReportItem item)
                    {
                        package.ReportItems.Add(item);
                    }
                }
            }
        }
    }
}

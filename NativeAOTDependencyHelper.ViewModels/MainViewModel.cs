using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Models;
using System.Collections.ObjectModel;

namespace NativeAOTDependencyHelper.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public IAsyncRelayCommand DotnetVersionCommand { get; } = new AsyncRelayCommand(DotnetToolingInterop.CheckDotnetVersion);

    [ObservableProperty]
    private ObservableCollection<NuGetPackageViewModel> _packages = new();

    // TODO: Have error string to report back issues initializing?

    [RelayCommand]
    public async Task ProcessSolutionFileAsync(string filepath)
    {
        // https://learn.microsoft.com/dotnet/core/tools/dotnet-list-package
        var dotnetPackageList = await DotnetToolingInterop.GetTransitiveDependencyListAsync(filepath);

        if (dotnetPackageList != null)
        {
            Packages = new(NuGetPackageInfo.FromJsonModels(dotnetPackageList).Select(p => new NuGetPackageViewModel(p)));
        }
    }
}

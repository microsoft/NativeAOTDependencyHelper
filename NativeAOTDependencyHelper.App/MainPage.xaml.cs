using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.JsonModels;
using NativeAOTDependencyHelper.Core.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NativeAOTDependencyHelper.App;

/// <summary>
/// Main page for our application, hosted in our <see cref="MainWindow"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    public IAsyncRelayCommand DotnetVersionCommand { get; } = new AsyncRelayCommand(DotnetToolingInterop.CheckDotnetVersion);

    public MainPage()
    {
        this.InitializeComponent();

        DotnetVersionCommand.Execute(this);
    }

    private async void OpenSolution_Click(object sender, RoutedEventArgs e)
    {
        // Create a file picker
        FileOpenPicker openPicker = new();

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, ((App)App.Current).MainWindowHwnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.List;
        openPicker.FileTypeFilter.Add(".sln");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();

        // https://learn.microsoft.com/dotnet/core/tools/dotnet-list-package
        var dotnetPackageList = await DotnetToolingInterop.GetTransitiveDependencyListAsync(file.Path);

        // TODO: x:Bind/Setup MainViewModel?
        DependencyView.ItemsSource = NuGetPackageViewModel.FromJsonModels(dotnetPackageList);
    }

    private static bool IsTaskSuccessful(TaskStatus status) => status == TaskStatus.RanToCompletion;
}

using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.ViewModels;
using NativeAOTDependencyHelper.ViewModels.Services;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NativeAOTDependencyHelper.App;

/// <summary>
/// Main page for our application, hosted in our <see cref="MainWindow"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = ((App)App.Current).Services.GetService<MainViewModel>();

    public BasicLogger Logger { get; private set; } = ((App)App.Current).Services.GetService<ILogger>() as BasicLogger;

    public MainPage()
    {
        InitializeComponent();

        ViewModel.DotnetVersionCommand.Execute(this);
    }

    private async void OpenSolution_Click(object sender, RoutedEventArgs e)
    {
        // Clear log
        Logger.Clear(); // TODO: Do we just make this part of the interface and handle in the MainViewModel?

        // Create a file picker
        FileOpenPicker openPicker = new();

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, ((App)App.Current).MainWindowHwnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.List;
        openPicker.FileTypeFilter.Add(".sln");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();

        await ViewModel.ProcessSolutionFileCommand.ExecuteAsync(file.Path);
    }

    private static bool IsTaskSuccessful(TaskStatus status) => status == TaskStatus.RanToCompletion;
}

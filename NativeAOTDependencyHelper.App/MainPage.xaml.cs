using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NativeAOTDependencyHelper.App;

/// <summary>
/// Main page for our application, hosted in our <see cref="MainWindow"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new(((App)App.Current).Services);

    public MainPage()
    {
        InitializeComponent();

        ViewModel.DotnetVersionCommand.Execute(this);
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

        await ViewModel.ProcessSolutionFileCommand.ExecuteAsync(file.Path);
    }

    private static bool IsTaskSuccessful(TaskStatus status) => status == TaskStatus.RanToCompletion;

    private void DependencyView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        DetailsController.SelectedPackage = (NuGetPackageViewModel)((sender as ItemsView).SelectedItem);
    }
}

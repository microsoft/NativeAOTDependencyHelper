using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NativeAOTDependencyHelper.App;

/// <summary>
/// Main page for our application, hosted in our <see cref="MainWindow"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        _ = InitializeDotnet();
    }

    // TODO: Put in the other project with the STJ parser
    private async Task InitializeDotnet()
    {
        try
        {
            Process process = new();
            process.StartInfo.FileName = "dotnet.exe";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            var version = await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            DotNetVersion.Text = $"Using .NET {version}";
        }
        catch (Exception ex)
        {
            DotNetVersion.Text = $"Couldn't find .NET in path: {ex.Message}";
        }
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
        // TODO: Pass to dotnet list <slnfilepath> package --include-transitive --format json
        // Pass into System.Text.Json...
    }
}

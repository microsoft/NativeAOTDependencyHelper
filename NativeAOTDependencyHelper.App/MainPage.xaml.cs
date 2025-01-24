// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Services;
using NativeAOTDependencyHelper.ViewModels;
using NativeAOTDependencyHelper.ViewModels.Services;
using System;
using Windows.Storage.Pickers;

namespace NativeAOTDependencyHelper.App;

/// <summary>
/// Main page for our application, hosted in our <see cref="MainWindow"/>.
/// </summary>
public sealed partial class MainPage : Page,
    IRecipient<LoggedErrorMessage>
{
    public MainViewModel ViewModel { get; } = ((App)App.Current).Services.GetService<MainViewModel>();

    public BasicLogger Logger { get; private set; } = ((App)App.Current).Services.GetService<ILogger>() as BasicLogger;

    public CredentialManager CredentialManager { get; } = ((App)App.Current).Services.GetService<CredentialManager>();

    private string _currentLoadPath = null; 

    public MainPage()
    {
        InitializeComponent();

        // Explicitly register our interaces as RegisterAll uses reflection
        WeakReferenceMessenger.Default.Register<LoggedErrorMessage>(this);

        ViewModel.DotnetVersionCommand.Execute(this);
    }

    public void Receive(LoggedErrorMessage message)
    {
        //OperationalLogExpander.IsExpanded = true;
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
        openPicker.FileTypeFilter.Add(".csproj");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();

        // If no file was selected, cancel operation
        if (file == null) return;

        // Caching this to enable project reload
        _currentLoadPath = file.Path;

        await ViewModel.ProcessSolutionFileCommand.ExecuteAsync(file.Path);
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        CredentialManager.WriteCredential(CredentialInputBox.Password);
        ViewModel.UpdateIsOpenSolutionEnabledProperty();
    }

    private async void ReloadSolution_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.OnProcessFinished();
        if (_currentLoadPath != null) await ViewModel.ProcessSolutionFileCommand.ExecuteAsync(_currentLoadPath);
    }

    private void DependencyView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // When a package is selected, hide the log view.
        LogToggleButton.IsChecked = false;
    }
}

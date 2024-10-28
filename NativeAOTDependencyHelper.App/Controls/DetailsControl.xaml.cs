using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NativeAOTDependencyHelper.ViewModels;

namespace NativeAOTDependencyHelper.App.Controls;

public sealed partial class DetailsControl : UserControl
{
    public DetailsControl()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register(nameof(SelectedPackage), typeof(NuGetPackageViewModel), typeof(DetailsControl), new PropertyMetadata(defaultValue: null));

    public NuGetPackageViewModel SelectedPackage
    {
        get => (NuGetPackageViewModel)GetValue(SelectedPackageProperty);
        set => SetValue(SelectedPackageProperty, value);
    }
}

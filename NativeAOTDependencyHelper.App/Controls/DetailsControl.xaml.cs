using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NativeAOTDependencyHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace NativeAOTDependencyHelper.App.Controls
{
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
}

// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(NuGetPackageViewModel), typeof(DetailsControl), new PropertyMetadata(defaultValue: null));

    public NuGetPackageViewModel ViewModel
    {
        get => (NuGetPackageViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}

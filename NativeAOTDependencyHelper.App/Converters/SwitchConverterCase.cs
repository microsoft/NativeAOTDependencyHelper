using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace NativeAOTDependencyHelper.App.Converters;

// TODO: Bring this to the WCT and just make Case work on DependencyObject and we can re-use them between the two...

/// <summary>
/// <see cref="SwitchConverterCase"/> is the value container for the <see cref="SwitchConverter"/>.
/// </summary>
[ContentProperty(Name = nameof(Content))]
public partial class SwitchConverterCase : DependencyObject
{
    /// <summary>
    /// Gets or sets the Content to display when this case is active.
    /// </summary>
    public DependencyObject Content
    {
        get { return (DependencyObject)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(UIElement), typeof(SwitchConverterCase), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets a value indicating whether this is the default case to display when no values match the specified value in the <see cref="SwitchPresenter"/>. There should only be a single default case provided. Do not set the <see cref="Value"/> property when setting <see cref="IsDefault"/> to <c>true</c>. Default is <c>false</c>.
    /// </summary>
    public bool IsDefault
    {
        get { return (bool)GetValue(IsDefaultProperty); }
        set { SetValue(IsDefaultProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDefault"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDefaultProperty =
        DependencyProperty.Register(nameof(IsDefault), typeof(bool), typeof(SwitchConverterCase), new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets the <see cref="Value"/> that this case represents. If it matches the <see cref="SwitchPresenter.Value"/> this case's <see cref="Content"/> will be displayed in the presenter.
    /// </summary>
    public object Value
    {
        get { return (object)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(object), typeof(SwitchConverterCase), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="Case"/> class.
    /// </summary>
    public SwitchConverterCase()
    {
    }
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using System;

namespace NativeAOTDependencyHelper.App.Converters;

/// <summary>
/// A helper <see cref="IValueConverter"/> which can automatically translate incoming data to a set of resulting values defined in XAML.
/// </summary>
/// <example>
/// &lt;converters:SwitchConverter x:Key=&quot;StatusToColorSwitchConverter&quot;
///                                TargetType=&quot;models:CheckStatus&quot;&gt;
///     &lt;controls:Case Value=&quot;Error&quot; Content=&quot;{ThemeResource SystemFillColorCriticalBrush}&quot;/&gt;
///     &lt;controls:Case Value=&quot;Warning&quot; Content=&quot;{ThemeResource SystemFillColorCautionBrush}&quot;/&gt;
///     &lt;controls:Case Value=&quot;Success&quot; Content=&quot;{ThemeResource SystemFillColorSuccessBrush}&quot;/&gt;
/// &lt;/converters:SwitchConverter&gt;
/// ...
/// &lt;TextBlock
///     FontWeight=&quot;SemiBold&quot;
///     Foreground=&quot;{x:Bind Status, Converter={StaticResource StatusToColorSwitchConverter}}&quot;
///     Text = &quot;{x:Bind Status}&quot; /&gt;
/// </example>
[ContentProperty(Name = nameof(SwitchCases))]
public class SwitchConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// Gets or sets a value representing the collection of cases to evaluate.
    /// </summary>
    public SwitchConverterCollection SwitchCases
    {
        get { return (SwitchConverterCollection)GetValue(SwitchCasesProperty); }
        set { SetValue(SwitchCasesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SwitchCases"/> property.
    /// </summary>
    public static readonly DependencyProperty SwitchCasesProperty =
        DependencyProperty.Register(nameof(SwitchCases), typeof(SwitchConverterCollection), typeof(SwitchConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets a value indicating which type to first cast and compare provided values against.
    /// </summary>
    public Type TargetType
    {
        get { return (Type)GetValue(TargetTypeProperty); }
        set { SetValue(TargetTypeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TargetType"/> property.
    /// </summary>
    public static readonly DependencyProperty TargetTypeProperty =
        DependencyProperty.Register(nameof(TargetType), typeof(Type), typeof(SwitchConverter), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchPresenter"/> class.
    /// </summary>
    public SwitchConverter()
    {
        // Note: we need to initialize this here so that XAML can automatically add cases without needing this defined around it as the content.
        // We don't do this in the PropertyMetadata as then we create a static shared collection for all converters, which we don't want. We want it per instance.
        // See https://learn.microsoft.com/windows/uwp/xaml-platform/custom-dependency-properties#initializing-the-collection
        SwitchCases = new SwitchConverterCollection();
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (SwitchCases == null ||
            SwitchCases.Count == 0)
        {
            // If we have no cases, then we can't match anything.
            return null;
        }

        SwitchConverterCase? xdefault = null;
        SwitchConverterCase? newcase = null;

        foreach (SwitchConverterCase xcase in SwitchCases)
        {
            if (xcase.IsDefault)
            {
                // If there are multiple default cases provided, this will override and just grab the last one, the developer will have to fix this in their XAML. We call this out in the case comments.
                xdefault = xcase;
                continue;
            }

            if (CompareValues(value, xcase.Value))
            {
                newcase = xcase;
                break;
            }
        }

        if (newcase == null && xdefault != null)
        {
            // Inject default if we found one without matching anything
            newcase = xdefault;
        }

        return newcase?.Content;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    // From: https://github.com/CommunityToolkit/Windows/blob/main/components/Primitives/src/SwitchPresenter/SwitchPresenter.cs

    /// <summary>
    /// Compares two values using the TargetType.
    /// </summary>
    /// <param name="compare">Our main value in our SwitchPresenter.</param>
    /// <param name="value">The value from the case to compare to.</param>
    /// <returns>true if the two values are equal</returns>
    private bool CompareValues(object compare, object value)
    {
        if (compare == null || value == null)
        {
            return compare == value;
        }

        if (TargetType == null ||
            (TargetType == compare.GetType() &&
             TargetType == value.GetType()))
        {
            // Default direct object comparison or we're all the proper type
            return compare.Equals(value);
        }
        else if (compare.GetType() == TargetType)
        {
            // If we have a TargetType and the first value is the right type
            // Then our 2nd value isn't, so convert to string and coerce.
            var valueBase2 = ConvertValue(TargetType, value);

            return compare.Equals(valueBase2);
        }

        // Neither of our two values matches the type so
        // we'll convert both to a String and try and coerce it to the proper type.
        var compareBase = ConvertValue(TargetType, compare);

        var valueBase = ConvertValue(TargetType, value);

        return compareBase.Equals(valueBase);
    }

    /// <summary>
    /// Helper method to convert a value from a source type to a target type.
    /// </summary>
    /// <param name="targetType">The target type</param>
    /// <param name="value">The value to convert</param>
    /// <returns>The converted value</returns>
    internal static object ConvertValue(Type targetType, object value)
    {
        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }
        else if (targetType.IsEnum && value is string str)
        {
#if HAS_UNO
            if (Enum.IsDefined(targetType, str))
            {
                return Enum.Parse(targetType, str);
            }
#else
            if (Enum.TryParse(targetType, str, out object? result))
            {
                return result!;
            }
#endif

            static object ThrowExceptionForKeyNotFound()
            {
                throw new InvalidOperationException("The requested enum value was not present in the provided type.");
            }

            return ThrowExceptionForKeyNotFound();
        }
        else
        {
            return XamlBindingHelper.ConvertValue(targetType, value);
        }
    }
}

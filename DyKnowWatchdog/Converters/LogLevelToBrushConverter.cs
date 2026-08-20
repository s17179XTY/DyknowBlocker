using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Converters;

/// <summary>Maps LogLevel to the preview's log-entry text color (Info green, Action red, Warn orange).</summary>
public sealed class LogLevelToBrushConverter : IValueConverter
{
    private static readonly Brush InfoBrush = Make("#34c759");
    private static readonly Brush ActionBrush = Make("#ff3b30");
    private static readonly Brush WarnBrush = Make("#ff9f0a");
    private static readonly Brush ErrorBrush = Make("#ff3b30");
    private static readonly Brush DefaultBrush = Make("#636366");

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            LogLevel.Info => InfoBrush,
            LogLevel.Action => ActionBrush,
            LogLevel.Warn => WarnBrush,
            LogLevel.Error => ErrorBrush,
            _ => DefaultBrush,
        };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;

    private static SolidColorBrush Make(string hex)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        brush.Freeze();
        return brush;
    }
}
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DyKnowWatchdog;

/// <summary>
/// Builds the system-tray icon as a WPF ImageSource (25% blue shield + status dot),
/// avoiding a System.Drawing dependency. Green dot = running, red dot = stopped.
/// </summary>
public static class TrayIconFactory
{
    private const int TraySize = 32;

    public static ImageSource Create(bool running)
    {
        var shield = new BitmapImage(new Uri("pack://application:,,,/Resources/app_256.png"));

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            dc.DrawImage(shield, new Rect(0, 0, TraySize, TraySize));

            var dotColor = running ? Color.FromRgb(0x34, 0xC7, 0x59) : Color.FromRgb(0xFF, 0x3B, 0x30);
            var dotBrush = new SolidColorBrush(dotColor);
            dotBrush.Freeze();
            var dotPen = new Pen(new SolidColorBrush(Color.FromArgb(0xE6, 0xFF, 0xFF, 0xFF)), 1.5);
            dotPen.Freeze();
            dc.DrawEllipse(dotBrush, dotPen, new Point(TraySize - 7, TraySize - 7), 6, 6);
        }

        var bitmap = new RenderTargetBitmap(TraySize, TraySize, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        bitmap.Freeze();
        return bitmap;
    }
}
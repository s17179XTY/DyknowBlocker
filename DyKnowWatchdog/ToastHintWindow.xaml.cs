using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DyKnowWatchdog.ViewModels;

namespace DyKnowWatchdog;

public partial class ToastHintWindow : Window
{
    private readonly DispatcherTimer _timer;

    public ToastHintWindow()
    {
        InitializeComponent();
        ShowInTaskbar = false;

        var work = SystemParameters.WorkArea;
        Left = work.Right - Width - 20;
        Top = work.Bottom - Height - 20;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _timer.Tick += (_, _) =>
        {
            _timer.Stop();
            var fade = new DoubleAnimation(Opacity, 0, TimeSpan.FromMilliseconds(600));
            fade.Completed += (_, _) => Close();
            BeginAnimation(OpacityProperty, fade);
        };
    }

    public void ShowToast(string text)
    {
        HintText.Text = text;
        Show();
        _timer.Start();
    }
}
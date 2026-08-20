using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DyKnowWatchdog.Services;
using DyKnowWatchdog.ViewModels;

namespace DyKnowWatchdog;

public partial class MainWindow : Window
{
    private MainViewModel? _vm;
    private bool _allowClose;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => WireViewModel();
    }

    private void WireViewModel()
    {
        if (DataContext is not MainViewModel vm)
            return;

        _vm = vm;

        // tray menu state
        TrayAutoStartItem.IsChecked = AutoStartRegistry.IsEnabled();
        UpdateTrayState(vm);

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(MainViewModel.IsRunning) or nameof(MainViewModel.Language))
                UpdateTrayState(vm);
        };

        // auto-scroll the log
        vm.Logs.CollectionChanged += (_, e) =>
        {
            if (e.NewItems is not null && LogList.Items.Count > 0)
                LogList.ScrollIntoView(LogList.Items[^1]);
        };

        // tray icon color follows state
        Tray.IconSource = TrayIconFactory.Create(vm.IsRunning);
    }

    private void UpdateTrayState(MainViewModel vm)
    {
        Tray.IconSource = TrayIconFactory.Create(vm.IsRunning);
        TrayToggleItem.Header = vm.StopButtonText;
        Tray.ToolTipText = vm.T.Title + " — " + vm.StatusText;
    }

    // -------- window drag (actual bar) --------
    private void WindowBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed)
            return;

        // only start drag from non-interactive area (mostly title bar / blank space)
        if (e.OriginalSource is DependencyObject dso &&
            (FindVisualParent<TextBox>(dso) != null || FindVisualParent<Button>(dso) != null))
            return;

        if (e.ClickCount == 1)
            DragMove();
    }

    private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null)
        {
            if (parent is T typed)
                return typed;
            parent = VisualTreeHelper.GetParent(parent);
        }
        return null;
    }

    // -------- close → tray (spec: 关闭窗口时最小化到托盘, 非退出) --------
    private void CloseBtn_Click(object sender, RoutedEventArgs e) => HideToTray();

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_allowClose)
            return;
        e.Cancel = true;
        HideToTray();
    }

    private void HideToTray()
    {
        Hide();
        // ensure the toast shows the tray hint once per app run (handled in App)
    }

    // -------- 监控频率 editing --------
    private void IntervalBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ApplyIntervalFromBox(sender);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            RevertInterval(sender);
            e.Handled = true;
        }
    }

    private void IntervalBox_LostFocus(object sender, RoutedEventArgs e) => ApplyIntervalFromBox(sender);

    private void ApplyIntervalFromBox(object sender)
    {
        if (sender is TextBox box && _vm is not null)
        {
            _vm.IntervalInput = box.Text;
            _vm.ApplyIntervalInput();
        }
    }

    private void RevertInterval(object sender)
    {
        if (sender is TextBox box && _vm is not null)
            box.Text = FrequencyParser.Format(_vm.IntervalSeconds);
    }

    // -------- tray --------
    private void Tray_DoubleClick(object sender, RoutedEventArgs e) => ShowWindow();

    private void Tray_OpenWindow(object sender, RoutedEventArgs e) => ShowWindow();

    private void Tray_Toggle(object sender, RoutedEventArgs e) => _vm?.Toggle();

    private void Tray_AutoStart(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item)
            AutoStartRegistry.SetEnabled(item.IsChecked);
    }

    private void Tray_Exit(object sender, RoutedEventArgs e)
    {
        _allowClose = true;
        Application.Current.Shutdown();
    }

    private void Tray_MouseMove(object sender, RoutedEventArgs e)
    {
        // tooltip kept fresh by UpdateTrayState
    }

    private void ShowWindow()
    {
        Show();
        if (WindowState == WindowState.Minimized)
            WindowState = WindowState.Normal;
        Activate();
        Topmost = true;
        Topmost = false;
        Focus();
    }
}
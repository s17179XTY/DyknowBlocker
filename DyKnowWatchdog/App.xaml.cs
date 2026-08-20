using System.IO;
using System.Windows;
using DyKnowWatchdog.Services;
using DyKnowWatchdog.ViewModels;

namespace DyKnowWatchdog;

public partial class App : Application
{
    private const string PreferredDyKnowRoot = @"C:\Program Files\DyKnow";

    private MainViewModel? _vm;
    private WatchdogService? _watchdog;
    private LogService? _logService;
    private MainWindow? _window;
    private ToastHintWindow? _toast;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown; // app lives in tray; only tray 退出 exits

        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DyKnowWatchdog");
        var settingsPath = Path.Combine(appData, "settings.json");
        var logsDir = Path.Combine(appData, "Logs");

        var settings = AppSettings.Load(settingsPath);
        _logService = new LogService(logsDir);

        // Directory discovery: prefer C:\Program Files\DyKnow\ → else DyKnow.exe 父父目录
        var monitorRoot = MonitorDirectoryLocator.Locate(PreferredDyKnowRoot);

        _watchdog = new WatchdogService(monitorRoot ?? "", new ProcessSource(), new ProcessKiller());
        _watchdog.Interval = TimeSpan.FromSeconds(settings.MonitoringIntervalSeconds);

        _vm = new MainViewModel(_watchdog, _logService, settings, settingsPath, monitorRoot);

        if (monitorRoot is null)
            _vm.HandleWatchdogEvent(new WatchdogEvent(WatchdogEventKind.ScanFailed, Detail: _vm.T.DirectoryNotFound));

        _window = new MainWindow { DataContext = _vm };
        MainWindow = _window;
        _window.Show();

        if (settings.AutoStartMonitoring && monitorRoot is not null)
            _watchdog.Start();

        // tray hint toast (preview bottom-right card) — show once shortly after launch
        _toast = new ToastHintWindow();
        _toast.ShowToast(_vm.T.TrayHint);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try { _watchdog?.Dispose(); } catch { }
        try { _vm?.Dispose(); } catch { }
        try { _logService?.Dispose(); } catch { }
        base.OnExit(e);
    }
}
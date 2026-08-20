using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.ViewModels;

public sealed class MainViewModel : ObservableObject, IDisposable
{
    private const int MaxLogEntries = 500;

    private readonly WatchdogService _watchdog;
    private readonly LogService _logService;
    private readonly AppSettings _settings;
    private readonly string _settingsPath;
    private readonly string? _monitorRoot;

    private IUiText _text = new ZhText();
    private string _language = "zh";
    private bool _isRunning;
    private string _intervalInput = "";

    public MainViewModel(
        WatchdogService watchdog,
        LogService logService,
        AppSettings settings,
        string settingsPath,
        string? monitorRoot)
    {
        _watchdog = watchdog ?? throw new ArgumentNullException(nameof(watchdog));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _settingsPath = settingsPath;
        _monitorRoot = monitorRoot;

        _language = settings.Language == "en" ? "en" : "zh";
        _text = _language == "en" ? new EnText() : new ZhText();
        _isRunning = _watchdog.IsRunning;
        _intervalInput = FrequencyParser.Format(_settings.MonitoringIntervalSeconds);

        _watchdog.Raised += OnWatchdogEvent;

        ToggleCommand = new RelayCommand(Toggle);
        RestartCommand = new RelayCommand(Restart);
        ClearLogCommand = new RelayCommand(ClearLog);
        OpenLogsCommand = new RelayCommand(OpenLogs);
        SetLanguageCommand = new RelayCommand(p => SetLanguage(p as string ?? "zh"));
    }

    public string Language { get => _language; private set => SetProperty(ref _language, value); }

    public IUiText T => _text;

    public string Version => "v1.0";

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (SetProperty(ref _isRunning, value))
            {
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(StopButtonText));
            }
        }
    }

    public string StatusText => IsRunning ? T.Running : T.Stopped;

    public string StopButtonText => IsRunning ? T.StopService : T.StartService;

    public string MonitorPath => _monitorRoot ?? T.DirectoryNotFound;

    public string IntervalInput
    {
        get => _intervalInput;
        set => SetProperty(ref _intervalInput, value);
    }

    /// <summary>Currently persisted interval, in seconds (used to revert invalid edits).</summary>
    public double IntervalSeconds => _settings.MonitoringIntervalSeconds;

    public ObservableCollection<LogEntryModel> Logs { get; } = new();

    public RelayCommand ToggleCommand { get; }
    public RelayCommand RestartCommand { get; }
    public RelayCommand ClearLogCommand { get; }
    public RelayCommand OpenLogsCommand { get; }
    public RelayCommand SetLanguageCommand { get; }

    public void Toggle()
    {
        if (IsRunning)
            _watchdog.Stop();
        else
            _watchdog.Start();
    }

    public void Restart()
    {
        if (IsRunning)
            _watchdog.Restart();
        else
            _watchdog.Start();
    }

    public void ClearLog() => Logs.Clear();

    public void OpenLogs()
    {
        try
        {
            Process.Start(new ProcessStartInfo("explorer.exe", _logService.LogsDirectory) { UseShellExecute = true });
        }
        catch
        {
            // ignore
        }
    }

    public void SetLanguage(string language)
    {
        var lang = language == "en" ? "en" : "zh";
        if (lang == _language) return;

        Language = lang;
        _text = lang == "en" ? new EnText() : new ZhText();
        _settings.Language = lang;
        _settings.Save(_settingsPath);

        OnPropertyChanged(nameof(T));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(StopButtonText));
        OnPropertyChanged(nameof(MonitorPath));
    }

    /// <summary>Parses + validates + persists the monitoring frequency. Returns true on success.</summary>
    public bool ApplyIntervalInput()
    {
        if (FrequencyParser.TryParse(IntervalInput, out var seconds))
        {
            _settings.MonitoringIntervalSeconds = seconds;
            _settings.Save(_settingsPath);
            _watchdog.Interval = TimeSpan.FromSeconds(seconds);
            var formatted = FrequencyParser.Format(seconds);
            IntervalInput = formatted;
            AddLog(LogLevel.Info, string.Format(T.IntervalChanged, formatted));
            return true;
        }

        var current = FrequencyParser.Format(_settings.MonitoringIntervalSeconds);
        IntervalInput = current;
        AddLog(LogLevel.Warn, string.Format(T.InvalidInterval, current));
        return false;
    }

    /// <summary>Public for testability; UI events also funnel here via the dispatcher.</summary>
    public void HandleWatchdogEvent(WatchdogEvent e)
    {
        switch (e.Kind)
        {
            case WatchdogEventKind.Started:
                IsRunning = true;
                AddLog(LogLevel.Info, T.MonitoringStarted);
                break;
            case WatchdogEventKind.Stopped:
                IsRunning = false;
                AddLog(LogLevel.Warn, T.MonitoringStopped);
                break;
            case WatchdogEventKind.Restarted:
                AddLog(LogLevel.Info, T.MonitoringRestarted);
                break;
            case WatchdogEventKind.Blocked:
                AddLog(LogLevel.Action, T.BlockedPrefix + DisplayName(e));
                break;
            case WatchdogEventKind.AccessDenied:
                AddLog(LogLevel.Warn, T.PermissionDeniedPrefix + DisplayName(e));
                break;
            case WatchdogEventKind.ScanFailed:
                AddLog(LogLevel.Error, e.Detail ?? "Scan failed");
                break;
        }
    }

    private static string DisplayName(WatchdogEvent e)
        => string.IsNullOrEmpty(e.ExecutablePath)
            ? e.ProcessName
            : Path.GetFileName(e.ExecutablePath);

    private void AddLog(LogLevel level, string message)
    {
        var time = DateTime.Now.ToString("HH:mm:ss");
        Logs.Add(new LogEntryModel($"[{time}]", message, level));
        if (Logs.Count > MaxLogEntries)
            Logs.RemoveAt(0);

        _logService.Write(level, message);
    }

    private void OnWatchdogEvent(WatchdogEvent e)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
            dispatcher.Invoke(() => HandleWatchdogEvent(e));
        else
            HandleWatchdogEvent(e);
    }

    public void Dispose()
    {
        _watchdog.Raised -= OnWatchdogEvent;
    }
}
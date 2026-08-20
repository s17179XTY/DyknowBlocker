using DyKnowWatchdog.Services;
using DyKnowWatchdog.ViewModels;

namespace DyKnowWatchdog.Tests;

public class MainViewModelTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _settingsPath;
    private readonly FakeProcessSource _source;
    private readonly FakeKiller _killer;
    private readonly WatchdogService _watchdog;
    private readonly LogService _log;
    private readonly AppSettings _settings;
    private readonly MainViewModel _vm;

    public MainViewModelTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "DyKnowVm_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        _settingsPath = Path.Combine(_tempDir, "settings.json");
        _source = new FakeProcessSource();
        _killer = new FakeKiller();
        _watchdog = new WatchdogService(_tempDir, _source, _killer);
        _log = new LogService(Path.Combine(_tempDir, "Logs"));
        _settings = new AppSettings();
        _vm = new MainViewModel(_watchdog, _log, _settings, _settingsPath, _tempDir);
    }

    public void Dispose()
    {
        _vm.Dispose();
        _watchdog.Dispose();
        _log.Dispose();
        try { if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true); }
        catch { /* best effort */ }
    }

    [Fact]
    public void Ctor_DefaultsToChinese()
    {
        Assert.Equal("zh", _vm.Language);
        Assert.Equal("DyKnow Watchdog", _vm.T.Title);
        Assert.Equal("监控目录", _vm.T.MonitorLabel);
        Assert.Equal("v1.0", _vm.Version);
        Assert.False(_vm.IsRunning);
    }

    [Fact]
    public void SetLanguage_English_SwitchesStrings()
    {
        _vm.SetLanguage("en");

        Assert.Equal("en", _vm.Language);
        Assert.Equal("Monitor Path", _vm.T.MonitorLabel);
        Assert.Equal("Stop Service", _vm.T.StopService);
        Assert.Equal("Running", _vm.T.Running);
    }

    [Fact]
    public void Toggle_StartsServiceAndRaisesState()
    {
        Assert.False(_vm.IsRunning);

        _vm.Toggle();

        Assert.True(_vm.IsRunning);
        Assert.True(_watchdog.IsRunning);
        Assert.Equal(_vm.T.StopService, _vm.StopButtonText);
        Assert.Equal(_vm.T.Running, _vm.StatusText);

        _vm.Toggle();
        Assert.False(_vm.IsRunning);
        Assert.False(_watchdog.IsRunning);
        Assert.Equal(_vm.T.StartService, _vm.StopButtonText);
        Assert.Equal(_vm.T.Stopped, _vm.StatusText);
    }

    [Fact]
    public void Restart_WhileRunning_LogsRestart()
    {
        _vm.Toggle();
        var before = _vm.Logs.Count;

        _vm.Restart();

        Assert.True(_vm.IsRunning);
        Assert.True(_vm.Logs.Count > before);
        Assert.Contains(_vm.Logs, l => l.Text.Contains(_vm.T.MonitoringRestarted));
    }

    [Fact]
    public void ApplyInterval_Valid_UpdatesServiceAndSettings()
    {
        _vm.Toggle(); // service running → interval change restarts timer
        var before = _vm.Logs.Count;

        _vm.IntervalInput = "2";
        var applied = _vm.ApplyIntervalInput();

        Assert.True(applied);
        Assert.Equal(TimeSpan.FromSeconds(2), _watchdog.Interval);
        Assert.Equal(2.0, _settings.MonitoringIntervalSeconds);
        Assert.Equal("2", _vm.IntervalInput);
        Assert.True(_vm.Logs.Count > before);
        Assert.Contains(_vm.Logs, l => l.Text.Contains("2s"));
    }

    [Fact]
    public void ApplyInterval_Invalid_RevertsInputAndLogsWarn()
    {
        _vm.IntervalInput = "abc";
        var applied = _vm.ApplyIntervalInput();

        Assert.False(applied);
        Assert.Equal("0.5", _vm.IntervalInput); // reverted to default
        Assert.Equal(0.5, _settings.MonitoringIntervalSeconds);
        Assert.Contains(_vm.Logs, l => l.Level == LogLevel.Warn);
    }

    [Fact]
    public void ApplyInterval_OutOfRange_Reverts()
    {
        _vm.IntervalInput = "9999";
        var applied = _vm.ApplyIntervalInput();

        Assert.False(applied);
        Assert.Equal("0.5", _vm.IntervalInput);
    }

    [Fact]
    public void HandleBlockedEvent_AddsActionLogWithFileName()
    {
        _vm.HandleWatchdogEvent(new WatchdogEvent(WatchdogEventKind.Blocked, "chrome",
            Path.Combine(_tempDir, "Client", "chrome.exe")));

        var entry = _vm.Logs.Last();
        Assert.Equal(LogLevel.Action, entry.Level);
        Assert.Contains(_vm.T.BlockedPrefix.TrimEnd(), entry.Text);
        Assert.Contains("chrome.exe", entry.Text);
    }

    [Fact]
    public void HandleAccessDeniedEvent_AddsWarnLog()
    {
        _vm.HandleWatchdogEvent(new WatchdogEvent(WatchdogEventKind.AccessDenied, "admin_tool",
            Path.Combine(_tempDir, "admin_tool.exe")));

        var entry = _vm.Logs.Last();
        Assert.Equal(LogLevel.Warn, entry.Level);
        Assert.Contains("admin_tool.exe", entry.Text);
    }

    [Fact]
    public void HandleStartedEvent_FlipsStateAndLogs()
    {
        _vm.HandleWatchdogEvent(new WatchdogEvent(WatchdogEventKind.Started));

        Assert.True(_vm.IsRunning);
        Assert.Contains(_vm.Logs, l => l.Text.Contains(_vm.T.MonitoringStarted));
    }

    [Fact]
    public void ClearLog_EmptiesCollection()
    {
        _vm.HandleWatchdogEvent(new WatchdogEvent(WatchdogEventKind.Started));
        Assert.NotEmpty(_vm.Logs);

        _vm.ClearLog();

        Assert.Empty(_vm.Logs);
    }

    [Fact]
    public void MonitorPath_NullRoot_ShowsDirectoryNotFound()
    {
        using var vm2 = new MainViewModel(_watchdog, _log, _settings, _settingsPath, null);

        Assert.Equal(_vm.T.DirectoryNotFound, vm2.MonitorPath);
    }

    private sealed class FakeProcessSource : IProcessSource
    {
        public List<ProcessSnapshot> Processes { get; } = new();
        public IReadOnlyList<ProcessSnapshot> GetProcesses() => Processes;
    }

    private sealed class FakeKiller : IProcessKiller
    {
        public List<int> KilledIds { get; } = new();
        public KillOutcome Outcome { get; set; } = KillOutcome.Killed;
        public KillOutcome TryKill(ProcessSnapshot process)
        {
            KilledIds.Add(process.Id);
            return Outcome;
        }
    }
}
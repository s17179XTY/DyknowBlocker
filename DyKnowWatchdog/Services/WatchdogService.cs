using System.Diagnostics;
using System.Threading;

namespace DyKnowWatchdog.Services;

/// <summary>
/// Core monitoring service: periodically scans running processes and kills every
/// process whose executable lives under the monitor root, except DyKnow.exe itself.
/// </summary>
public sealed class WatchdogService : IDisposable
{
    public const double MinIntervalSeconds = 0.1;
    public const double MaxIntervalSeconds = 3600.0;

    private readonly string _monitorRoot;
    private readonly IProcessSource _processSource;
    private readonly IProcessKiller _processKiller;
    private readonly object _sync = new();
    private Timer? _timer;
    private int _scanning;
    private TimeSpan _interval = TimeSpan.FromMilliseconds(500);

    public WatchdogService(string monitorRoot, IProcessSource processSource, IProcessKiller processKiller)
    {
        _monitorRoot = monitorRoot;
        _processSource = processSource ?? throw new ArgumentNullException(nameof(processSource));
        _processKiller = processKiller ?? throw new ArgumentNullException(nameof(processKiller));
    }

    public string MonitorRoot => _monitorRoot;

    public bool IsRunning { get; private set; }

    /// <summary>Monitoring frequency. Clamped to [0.1s, 3600s]. Restarts the timer when running.</summary>
    public TimeSpan Interval
    {
        get => _interval;
        set
        {
            var clamped = ClampInterval(value);
            lock (_sync)
            {
                if (clamped == _interval)
                    return;
                _interval = clamped;
                if (IsRunning)
                    RestartTimerLocked();
            }
        }
    }

    /// <summary>Raised for every noteworthy event (start/stop/blocked/denied/…). UI subscribes.</summary>
    public event Action<WatchdogEvent>? Raised;

    public void Start()
    {
        lock (_sync)
        {
            if (IsRunning)
                return;
            IsRunning = true;
            StartTimerLocked();
        }
        Raise(WatchdogEventKind.Started);
    }

    public void Stop()
    {
        lock (_sync)
        {
            if (!IsRunning)
                return;
            IsRunning = false;
            _timer?.Dispose();
            _timer = null;
        }
        Raise(WatchdogEventKind.Stopped);
    }

    /// <summary>Stops then starts the service (used by the "重启" button).</summary>
    public void Restart()
    {
        var wasRunning = IsRunning;
        Stop();
        if (wasRunning)
            Start();
        Raise(WatchdogEventKind.Restarted);
    }

    /// <summary>
    /// One full scan: enumerate processes, kill every non-DyKnow.exe process whose
    /// executable path is within the monitor root. Callable directly from tests.
    /// </summary>
    public void ScanOnce()
    {
        IReadOnlyList<ProcessSnapshot> processes;
        try
        {
            processes = _processSource.GetProcesses();
        }
        catch (Exception ex)
        {
            Raise(WatchdogEventKind.ScanFailed, detail: ex.Message);
            return;
        }

        foreach (var p in processes)
        {
            if (p.ProcessName.Equals("DyKnow", StringComparison.OrdinalIgnoreCase))
                continue; // never kill DyKnow.exe itself
            if (string.IsNullOrEmpty(p.ExecutablePath))
                continue;
            if (!PathUtil.IsWithinDirectory(_monitorRoot, p.ExecutablePath))
                continue;

            var outcome = _processKiller.TryKill(p);
            switch (outcome)
            {
                case KillOutcome.Killed:
                    Raise(WatchdogEventKind.Blocked, p.ProcessName, p.ExecutablePath);
                    break;
                case KillOutcome.AccessDenied:
                    Raise(WatchdogEventKind.AccessDenied, p.ProcessName, p.ExecutablePath);
                    break;
                case KillOutcome.AlreadyExited:
                    break; // raced with process exit — silently ignore
            }
        }
    }

    private void TimerTick(object? state)
    {
        if (!IsRunning)
            return;
        if (Interlocked.CompareExchange(ref _scanning, 1, 0) != 0)
            return; // previous scan still running — skip this tick

        try
        {
            ScanOnce();
        }
        finally
        {
            Interlocked.Exchange(ref _scanning, 0);
        }
    }

    private void StartTimerLocked()
    {
        _timer?.Dispose();
        var ms = (long)Math.Max(50, _interval.TotalMilliseconds);
        _timer = new Timer(TimerTick, null, ms, ms);
    }

    private void RestartTimerLocked()
    {
        if (IsRunning)
            StartTimerLocked();
    }

    private static TimeSpan ClampInterval(TimeSpan value)
    {
        var seconds = Math.Clamp(value.TotalSeconds, MinIntervalSeconds, MaxIntervalSeconds);
        return TimeSpan.FromSeconds(seconds);
    }

    private void Raise(WatchdogEventKind kind, string processName = "", string? exePath = null, string? detail = null)
    {
        try
        {
            Raised?.Invoke(new WatchdogEvent(kind, processName, exePath, detail));
        }
        catch
        {
            // a faulty subscriber must not kill the watchdog thread
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            IsRunning = false;
            _timer?.Dispose();
            _timer = null;
        }
    }
}
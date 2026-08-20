namespace DyKnowWatchdog.Services;

/// <summary>A snapshot of a running process (testable abstraction over System.Diagnostics.Process).</summary>
public sealed record ProcessSnapshot(int Id, string ProcessName, string? ExecutablePath);

public enum LogLevel { Info, Action, Warn, Error }

/// <summary>Destination for log lines (file, UI, tests).</summary>
public interface ILogSink
{
    void Write(LogLevel level, string message);
}

public enum KillOutcome { Killed, AccessDenied, AlreadyExited }

public interface IProcessKiller
{
    KillOutcome TryKill(ProcessSnapshot process);
}

public interface IProcessSource
{
    IReadOnlyList<ProcessSnapshot> GetProcesses();
}

public enum WatchdogEventKind
{
    Started,
    Stopped,
    Restarted,
    Blocked,
    AccessDenied,
    ScanFailed,
}

public sealed record WatchdogEvent(
    WatchdogEventKind Kind,
    string ProcessName = "",
    string? ExecutablePath = null,
    string? Detail = null);
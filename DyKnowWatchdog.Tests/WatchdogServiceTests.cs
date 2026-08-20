using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Tests;

public class WatchdogServiceTests : IDisposable
{
    private readonly string _tempRoot;

    public WatchdogServiceTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "DyKnowWd_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempRoot);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_tempRoot)) Directory.Delete(_tempRoot, true); }
        catch { /* best effort */ }
    }

    private string UnderRoot(string relative)
    {
        var p = Path.Combine(_tempRoot, relative.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(p)!);
        // only materialize actual files for exe-like paths; pure directories stay virtual
        if (Path.GetFileName(p).Contains('.'))
            File.WriteAllText(p, "");
        return p;
    }

    private static ProcessSnapshot Proc(int id, string name, string? exe) => new(id, name, exe);

    private static (WatchdogService svc, FakeProcessSource source, FakeKiller killer, List<WatchdogEvent> events)
        CreateService(string root)
    {
        var source = new FakeProcessSource();
        var killer = new FakeKiller();
        var svc = new WatchdogService(root, source, killer);
        var events = new List<WatchdogEvent>();
        svc.Raised += e => events.Add(e);
        return (svc, source, killer, events);
    }

    [Fact]
    public void ScanOnce_KillsNonDyKnowProcessesUnderRoot()
    {
        var root = UnderRoot("DyKnow");
        var (svc, source, killer, events) = CreateService(root);
        var dyKnowExe = UnderRoot("DyKnow\\Client\\DyKnow.exe");
        var chromeExe = UnderRoot("DyKnow\\Client\\chrome.exe");
        var outsideExe = UnderRoot("Other\\notepad.exe");
        source.Processes.Add(Proc(1, "DyKnow", dyKnowExe));
        source.Processes.Add(Proc(2, "chrome", chromeExe));
        source.Processes.Add(Proc(3, "notepad", outsideExe));

        svc.ScanOnce();

        Assert.Equal(new[] { 2 }, killer.KilledIds);
        Assert.Contains(events, e => e.Kind == WatchdogEventKind.Blocked && e.ProcessName == "chrome");
        Assert.DoesNotContain(events, e => e.Kind == WatchdogEventKind.Blocked && e.ProcessName == "DyKnow");
        Assert.DoesNotContain(events, e => e.Kind == WatchdogEventKind.Blocked && e.ProcessName == "notepad");
    }

    [Fact]
    public void ScanOnce_SkipsDyKnowItself_WhenOnlyProcessUnderRoot()
    {
        var (svc, source, killer, events) = CreateService(_tempRoot);
        var dyKnowExe = UnderRoot("DyKnow.exe");
        source.Processes.Add(Proc(1, "DyKnow", dyKnowExe));

        svc.ScanOnce();

        Assert.Empty(killer.KilledIds);
        Assert.DoesNotContain(events, e => e.Kind == WatchdogEventKind.Blocked);
    }

    [Fact]
    public void ScanOnce_SkipsNullAndOutsideExecutablePaths()
    {
        var (svc, source, killer, _) = CreateService(_tempRoot);
        source.Processes.Add(Proc(1, "svchost", null));
        source.Processes.Add(Proc(2, "app2", @"C:\Windows\System32\app2.exe"));

        svc.ScanOnce();

        Assert.Empty(killer.KilledIds);
    }

    [Fact]
    public void ScanOnce_AccessDenied_LogsWarningAndContinues()
    {
        var (svc, source, killer, events) = CreateService(_tempRoot);
        var app = UnderRoot("Client\\admin_tool.exe");
        source.Processes.Add(Proc(1, "admin_tool", app));
        source.Processes.Add(Proc(2, "app2", UnderRoot("app2.exe")));
        killer.Outcome = KillOutcome.AccessDenied;

        svc.ScanOnce();

        Assert.Equal(new[] { 1, 2 }, killer.KilledIds); // still attempted for both
        Assert.Equal(2, events.Count(e => e.Kind == WatchdogEventKind.AccessDenied));
        Assert.Equal(0, events.Count(e => e.Kind == WatchdogEventKind.Blocked));
    }

    [Fact]
    public void ScanOnce_AlreadyExited_NoEvents()
    {
        var (svc, source, killer, events) = CreateService(_tempRoot);
        var app = UnderRoot("app.exe");
        source.Processes.Add(Proc(1, "app", app));
        killer.Outcome = KillOutcome.AlreadyExited;

        svc.ScanOnce();

        Assert.Empty(events);
        Assert.Equal(new[] { 1 }, killer.KilledIds);
    }

    [Fact]
    public void Interval_SetClampsToAllowedBounds()
    {
        var (svc, _, _, _) = CreateService(_tempRoot);

        svc.Interval = TimeSpan.FromMilliseconds(50);
        Assert.Equal(TimeSpan.FromMilliseconds(100), svc.Interval);

        svc.Interval = TimeSpan.FromHours(2);
        Assert.Equal(TimeSpan.FromSeconds(3600), svc.Interval);

        svc.Interval = TimeSpan.FromMilliseconds(1500);
        Assert.Equal(TimeSpan.FromMilliseconds(1500), svc.Interval);
    }

    [Fact]
    public void StartStop_RaisesLifecycleEvents()
    {
        var (svc, _, _, events) = CreateService(_tempRoot);

        svc.Start();
        Assert.True(svc.IsRunning);
        svc.Stop();
        Assert.False(svc.IsRunning);

        Assert.Contains(events, e => e.Kind == WatchdogEventKind.Started);
        Assert.Contains(events, e => e.Kind == WatchdogEventKind.Stopped);
    }

    [Fact]
    public void Start_IsIdempotent()
    {
        var (svc, _, _, events) = CreateService(_tempRoot);

        svc.Start();
        svc.Start();
        svc.Stop();
        svc.Stop();

        Assert.Equal(1, events.Count(e => e.Kind == WatchdogEventKind.Started));
        Assert.Equal(1, events.Count(e => e.Kind == WatchdogEventKind.Stopped));
    }

    [Fact]
    public void Interval_RestartsTimerWhenRunning()
    {
        var (svc, _, _, _) = CreateService(_tempRoot);
        svc.Start();
        var before = svc.Interval;

        svc.Interval = TimeSpan.FromSeconds(2);

        Assert.NotEqual(before, svc.Interval);
        Assert.True(svc.IsRunning);
        svc.Dispose();
        Assert.False(svc.IsRunning);
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
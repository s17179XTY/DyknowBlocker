using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Tests;

public class LogServiceTests : IDisposable
{
    private readonly string _tempDir;

    public LogServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "DyKnowLog_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true); }
        catch { /* best effort */ }
    }

    [Fact]
    public void Write_CreatesDailyFileWithMessage()
    {
        var clock = new DateTime(2026, 8, 2, 14, 30, 1);
        using var log = new LogService(_tempDir, clock: () => clock);

        log.Write(LogLevel.Info, "已启动监控");

        var file = Path.Combine(_tempDir, "260802.log");
        Assert.True(File.Exists(file));
        var content = File.ReadAllText(file);
        Assert.Contains("已启动监控", content);
        Assert.Contains("INFO", content);
    }

    [Fact]
    public void Write_AppendsLines()
    {
        using var log = new LogService(_tempDir);
        log.Write(LogLevel.Info, "one");
        log.Write(LogLevel.Action, "two");

        var content = File.ReadAllText(Directory.GetFiles(_tempDir, "*.log").Single());
        Assert.Contains("one", content);
        Assert.Contains("two", content);
        Assert.Equal(2, content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length);
    }

    [Fact]
    public void Write_WhenFileExceedsMaxBytes_RotatesToSuffixedFile()
    {
        using var log = new LogService(_tempDir, maxFileBytes: 100);

        for (int i = 0; i < 20; i++)
            log.Write(LogLevel.Info, new string('x', 30));

        var files = Directory.GetFiles(_tempDir, "*.log");
        Assert.True(files.Length >= 2, $"expected rotation, got {files.Length} file(s)");
    }

    [Fact]
    public void PurgeExpired_DeletesOldFiles()
    {
        var clock = new DateTime(2026, 8, 2, 14, 30, 0);
        using var log = new LogService(_tempDir, retentionDays: 30, clock: () => clock);

        var old = Path.Combine(_tempDir, "260101.log");
        File.WriteAllText(old, "old");
        File.SetLastWriteTime(old, clock.AddDays(-31));

        log.Write(LogLevel.Info, "trigger");

        Assert.False(File.Exists(old));
    }

    [Fact]
    public void PurgeExpired_KeepsRecentFiles()
    {
        var clock = new DateTime(2026, 8, 2, 14, 30, 0);
        using var log = new LogService(_tempDir, retentionDays: 30, clock: () => clock);

        var recent = Path.Combine(_tempDir, "260801.log");
        File.WriteAllText(recent, "recent");
        File.SetLastWriteTime(recent, clock.AddDays(-1));

        log.Write(LogLevel.Info, "trigger");

        Assert.True(File.Exists(recent));
    }
}
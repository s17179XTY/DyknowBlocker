using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Tests;

public class AppSettingsTests : IDisposable
{
    private readonly string _tempDir;

    public AppSettingsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "DyKnowSet_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true); }
        catch { /* best effort */ }
    }

    [Fact]
    public void Save_ThenLoad_RoundTrips()
    {
        var path = Path.Combine(_tempDir, "settings.json");
        var original = new AppSettings { MonitoringIntervalSeconds = 2.5, AutoStartMonitoring = false, Language = "en" };

        original.Save(path);
        var loaded = AppSettings.Load(path);

        Assert.Equal(2.5, loaded.MonitoringIntervalSeconds);
        Assert.False(loaded.AutoStartMonitoring);
        Assert.Equal("en", loaded.Language);
    }

    [Fact]
    public void Load_WhenFileMissing_ReturnsDefaults()
    {
        var path = Path.Combine(_tempDir, "missing.json");

        var loaded = AppSettings.Load(path);

        Assert.Equal(0.5, loaded.MonitoringIntervalSeconds);
        Assert.True(loaded.AutoStartMonitoring);
        Assert.Equal("zh", loaded.Language);
    }

    [Fact]
    public void Load_WhenFileCorrupt_ReturnsDefaults()
    {
        var path = Path.Combine(_tempDir, "corrupt.json");
        File.WriteAllText(path, "{ not json !!!");

        var loaded = AppSettings.Load(path);

        Assert.Equal(0.5, loaded.MonitoringIntervalSeconds);
    }
}
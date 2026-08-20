using System.Globalization;
using System.IO;

namespace DyKnowWatchdog.Services;

/// <summary>
/// File-backed logger.
/// <para>
/// - Directory: passed in (production: %LOCALAPPDATA%\DyKnowWatchdog\Logs)
/// - File name: yyMMdd.log (daily split), rotation appends _001, _002… when a file
///   reaches <see cref="MaxFileBytes"/>.
/// - Retention: files older than <see cref="RetentionDays"/> are purged on each write.
/// </para>
/// </summary>
public sealed class LogService : ILogSink, IDisposable
{
    private const long DefaultMaxFileBytes = 5L * 1024 * 1024; // 5 MB
    private readonly string _logsDirectory;
    private readonly int _retentionDays;
    private readonly long _maxFileBytes;
    private readonly Func<DateTime> _clock;

    public LogService(
        string logsDirectory,
        int retentionDays = 30,
        long maxFileBytes = DefaultMaxFileBytes,
        Func<DateTime>? clock = null)
    {
        _logsDirectory = logsDirectory;
        _retentionDays = retentionDays;
        _maxFileBytes = maxFileBytes;
        _clock = clock ?? (() => DateTime.Now);
    }

    public string LogsDirectory => _logsDirectory;

    public void Write(LogLevel level, string message)
    {
        try
        {
            Directory.CreateDirectory(_logsDirectory);
            PurgeExpired();

            var target = GetTargetPath();
            var line = string.Create(CultureInfo.InvariantCulture,
                $"[{_clock():yyyy-MM-dd HH:mm:ss}] [{level.ToString().ToUpperInvariant()}] {message}");
            File.AppendAllText(target, line + Environment.NewLine);
        }
        catch
        {
            // logging must never crash the watchdog
        }
    }

    /// <summary>Deletes log files whose last write time is older than the retention window.</summary>
    public void PurgeExpired()
    {
        if (!Directory.Exists(_logsDirectory))
            return;

        var cutoff = _clock().AddDays(-_retentionDays);
        foreach (var file in Directory.EnumerateFiles(_logsDirectory, "*.log"))
        {
            try
            {
                if (File.GetLastWriteTime(file) < cutoff)
                    File.Delete(file);
            }
            catch
            {
                // file locked or deleted concurrently — best effort
            }
        }
    }

    private string GetTargetPath()
    {
        var basePath = Path.Combine(_logsDirectory, _clock().ToString("yyMMdd") + ".log");
        if (!File.Exists(basePath) || new FileInfo(basePath).Length < _maxFileBytes)
            return basePath;

        for (var i = 1; i < 10000; i++)
        {
            var rotated = Path.Combine(_logsDirectory, $"{_clock():yyMMdd}_{i:000}.log");
            if (!File.Exists(rotated) || new FileInfo(rotated).Length < _maxFileBytes)
                return rotated;
        }
        return basePath;
    }

    public void Dispose()
    {
        // AppendAllText opens/closes per write; nothing to release.
    }
}
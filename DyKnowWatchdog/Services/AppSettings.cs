using System.IO;
using System.Text.Json;

namespace DyKnowWatchdog.Services;

/// <summary>App settings persisted as JSON (production: %LOCALAPPDATA%\DyKnowWatchdog\settings.json).</summary>
public sealed class AppSettings
{
    public double MonitoringIntervalSeconds { get; set; } = FrequencyParser.DefaultSeconds;
    public bool AutoStartMonitoring { get; set; } = true;
    public string Language { get; set; } = "zh";

    public void Save(string settingsPath)
    {
        try
        {
            var dir = Path.GetDirectoryName(settingsPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // best effort
        }
    }

    public static AppSettings Load(string settingsPath)
    {
        try
        {
            if (!File.Exists(settingsPath))
                return new AppSettings();
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsPath)) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }
}
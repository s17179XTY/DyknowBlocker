using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.ViewModels;

/// <summary>One scrollable log line shown in the UI.</summary>
public sealed class LogEntryModel
{
    public LogEntryModel(string time, string text, LogLevel level)
    {
        Time = time;
        Text = text;
        Level = level;
    }

    public string Time { get; }
    public string Text { get; }
    public LogLevel Level { get; }
}
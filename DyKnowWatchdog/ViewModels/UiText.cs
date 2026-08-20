namespace DyKnowWatchdog.ViewModels;

/// <summary>All user-facing strings, localized (mirrors the preview's T object).</summary>
public interface IUiText
{
    string Title { get; }
    string Running { get; }
    string Stopped { get; }
    string MonitorLabel { get; }
    string IntervalLabel { get; }
    string StopService { get; }
    string StartService { get; }
    string Restart { get; }
    string ViewLogs { get; }
    string LogsLabel { get; }
    string Clear { get; }
    string TrayHint { get; }
    string BlockedPrefix { get; }
    string PermissionDeniedPrefix { get; }
    string MonitoringStarted { get; }
    string MonitoringStopped { get; }
    string MonitoringRestarted { get; }
    string DirectoryNotFound { get; }
    string InvalidInterval { get; }
    string IntervalChanged { get; }
}

public sealed class ZhText : IUiText
{
    public string Title => "DyKnow Watchdog";
    public string Running => "运行中";
    public string Stopped => "已停止";
    public string MonitorLabel => "监控目录";
    public string IntervalLabel => "监控间隔";
    public string StopService => "停止服务";
    public string StartService => "启动服务";
    public string Restart => "重启";
    public string ViewLogs => "📋 查看日志";
    public string LogsLabel => "日志";
    public string Clear => "清除";
    public string TrayHint => "关闭时最小化到托盘";
    public string BlockedPrefix => "阻止: ";
    public string PermissionDeniedPrefix => "权限不足，跳过: ";
    public string MonitoringStarted => "已启动监控";
    public string MonitoringStopped => "已停止监控";
    public string MonitoringRestarted => "已重启监控";
    public string DirectoryNotFound => "未找到 DyKnow 目录";
    public string InvalidInterval => "无效的监控间隔，已还原为 {0}s";
    public string IntervalChanged => "监控间隔已改为 {0}s";
}

public sealed class EnText : IUiText
{
    public string Title => "DyKnow Watchdog";
    public string Running => "Running";
    public string Stopped => "Stopped";
    public string MonitorLabel => "Monitor Path";
    public string IntervalLabel => "Interval";
    public string StopService => "Stop Service";
    public string StartService => "Start Service";
    public string Restart => "Restart";
    public string ViewLogs => "📋 View Logs";
    public string LogsLabel => "Log";
    public string Clear => "Clear";
    public string TrayHint => "Minimize to tray on close";
    public string BlockedPrefix => "Blocked: ";
    public string PermissionDeniedPrefix => "No permission, skipped: ";
    public string MonitoringStarted => "Monitoring started";
    public string MonitoringStopped => "Monitoring stopped";
    public string MonitoringRestarted => "Monitoring restarted";
    public string DirectoryNotFound => "DyKnow directory not found";
    public string InvalidInterval => "Invalid interval, reverted to {0}s";
    public string IntervalChanged => "Interval changed to {0}s";
}
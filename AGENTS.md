# AGENTS.md

本文件为后续在此仓库工作的 Agent 提供项目背景、架构与开发约定。所有新增代码请先阅读本文再动手。

## 1. 项目是什么

**DyKnow Watchdog**（仓库名 DyknowBlocker）：一个 Windows 桌面工具，监控
`C:\Program Files\DyKnow\` 目录树下的所有进程，并**终止其中除 DyKnow.exe 本体外的所有进程**。
用途是绕过/阻断 DyKnow 课堂监控软件的进程（对应旧版 WinForms 版 Blocker 的演进重写）。

> 旧实现分叉点：`git tag v1.0..v1.2`（WinForms，`Blocker/` 目录）。本仓库
> `master` 分支仍保留旧实现；新的 .NET 9 WPF 实现在 `feature/dyknow-watchdog` →
> `main`。

## 2. 技术栈

- C# 12 / .NET 9.0 / WPF (`net9.0-windows`)
- `Hardcodet.NotifyIcon.Wpf`（系统托盘）
- xUnit（测试）
- 无 MVVM 框架依赖（手写 `ObservableObject` + `RelayCommand`）

## 3. 目录结构

```
DyKnowWatchdog.sln
DyKnowWatchdog/                  # 主程序 (WinExe, net9.0-windows)
├── App.xaml(.cs)               # 组合根：组装 Settings/LogService/WatchdogService/VM/Window
├── MainWindow.xaml(.cs)        # UI（与 docs/superpowers/specs/dyknow-preview.html 逐像素一致）
├── ToastHintWindow.xaml(.cs)   # 托盘提示 toast（右下角，4 秒自动消失）
├── TrayIconFactory.cs          # 用 WPF 绘制托盘图标（绿=运行 / 红=停止），无 System.Drawing 依赖
├── Converters/                 # LogLevel → 颜色
├── ViewModels/                 # MainViewModel（双语、日志集合、命令、频率解析）
├── Services/
│   ├── WatchdogService.cs      # 核心：定时扫描 + 杀进程 + 事件
│   ├── MonitorDirectoryLocator.cs  # 目录发现（见 §5）
│   ├── FrequencyParser.cs      # 频率解析/格式化（0.1–3600s）
│   ├── LogService.cs           # 文件日志：每日分割 / 5MB 轮换 / 30 天清理
│   ├── AppSettings.cs          # settings.json 持久化
│   ├── AutoStartRegistry.cs    # HKCU Run 开机自启
│   ├── PathUtil.cs             # 路径包含判断
│   ├── Models.cs               # ProcessSnapshot / 接口 / 事件类型
│   ├── ProcessSource.cs        # IProcessSource: 枚举真实进程
│   └── ProcessKiller.cs        # IProcessKiller: Process.Kill 封装
└── Resources/                  # app.ico + 各尺寸 PNG
DyKnowWatchdog.Tests/            # xUnit（71 个用例）
```

## 4. 核心架构与可测试性

**关键设计：所有系统调用都经接口抽象，WatchdogService 不直接碰 `System.Diagnostics.Process`。**

```
IProcessSource  ──枚举进程──▶  List<ProcessSnapshot(Id, ProcessName, ExecutablePath)>
IProcessKiller  ──杀进程────▶  KillOutcome (Killed / AccessDenied / AlreadyExited)
WatchdogService ──事件──────▶  Action<WatchdogEvent> Raised  (timer 线程触发)
```

- `WatchdogService.Start()/Stop()/Restart()` 管理 `System.Threading.Timer`；
  `Interval` 可改（clamp 到 0.1–3600s，运行中修改会重建 timer）。
- `ScanOnce()` 是**公开**的——测试直接调用它，不依赖 timer。
- 杀进程规则：`ProcessName != "DyKnow"`（大小写不敏感）且
  `ExecutablePath` 非空且 `PathUtil.IsWithinDirectory(monitorRoot, path)`。
  权限不足 → `AccessDenied` 事件（UI 显示「权限不足，跳过」），进程已退出 → 静默。
- 事件在 timer 线程触发；`MainViewModel.OnWatchdogEvent` 里会
  `Dispatcher.Invoke` 切回 UI 线程（`Application.Current?.Dispatcher`）。
- **绝不允许**在 UI 线程做扫描/杀进程；**绝不允许**在事件回调里抛异常
  （`Raise` 已包 try/catch，但订阅方仍要保持轻量）。

## 5. 目录发现规则（用户明确要求）

1. 优先 `C:\Program Files\DyKnow\` ——存在即用（本机即此情形）。
2. 否则用 `Process.GetProcessesByName("DyKnow")` 拿 DyKnow.exe 的
   `MainModule.FileName`，从 exe 所在目录向上找**最深的名称含 "DyKnow" 的目录**
   （即「父父目录」：`<root>\Client\DyKnow.exe` → `<root>`）。
3. 都找不到 → `null`，UI 的监控目录显示「未找到 DyKnow 目录」。

实现见 `MonitorDirectoryLocator`，测试见 `MonitorDirectoryLocatorTests`。
**注意**：测试的临时目录名不要包含 "DyKnow" 字样，否则会干扰该启发式。

## 6. 监控频率（用户明确要求：可配置）

- 默认 0.5s；合法范围 0.1–3600s，支持小数秒（如 `0.5`、`2`、`120`）。
- UI：监控间隔卡片内的输入框直接编辑，`Enter`/失焦生效；非法输入回退并记 Warn。
- 持久化：`%LOCALAPPDATA%\DyKnowWatchdog\settings.json` 的 `MonitoringIntervalSeconds`。
- 解析/校验：`FrequencyParser`（拒绝指数、多小数点、越界）。

## 7. 日志

- 目录 `%LOCALAPPDATA%\DyKnowWatchdog\Logs\`
- 文件名 `yyMMdd.log`（每日分割）；超 5MB 轮换为 `yyMMdd_001.log`…；30 天前文件自动清理。
- 行格式 `[yyyy-MM-dd HH:mm:ss] [LEVEL] 消息`；UI 日志 = 内存列表（最多 500 条）+ 同一份写入文件。
- 颜色：Info 绿 / Action 红（阻止）/ Warn 橙（权限不足）/ Error 红。

## 8. 托盘与行为

- 关闭窗口（✕ / Alt+F4）→ **最小化到托盘**，进程不退出（spec 3.3）。
- 托盘图标：运行绿点 / 停止红点（`TrayIconFactory`，WPF 绘制，无 System.Drawing）。
- 托盘右键：打开窗口 / 启动·停止服务（文案随状态切换）/ 开机自启动（HKCU Run，勾选）/ 退出。
- 托盘双击恢复窗口。
- 开机自启：`AutoStartRegistry`（`HKCU\...\CurrentVersion\Run`，值名 `DyKnowWatchdog`）。

## 9. 中英文 UI

- `ViewModels/UiText.cs`：`IUiText` 接口 + `ZhText` / `EnText` 两个实现（对应预览 HTML 里的 `T` 对象）。
- `MainViewModel.T` 暴露当前语言文本；语言保存在 settings.json；历史日志条目不重译。
- 新增文案必须同时加 zh 和 en。

## 10. 构建 / 测试 / 运行

```powershell
dotnet build DyKnowWatchdog.sln          # 0 警告 0 错误是硬性要求
dotnet test DyKnowWatchdog.sln           # 71 个用例全绿
# 运行：
dotnet run --project DyKnowWatchdog
# 或直接执行
DyKnowWatchdog\bin\Debug\net9.0-windows\DyKnowWatchdog.exe
```

项目配置 `LangVersion=12`、`ImplicitUsings` 启用。注意 WPF 编译环境下
`System.IO` / `System.Diagnostics` 等**不会自动引入**——用到 `Path`、`File`、
`Process` 时必须显式 `using System.IO;` / `using System.Diagnostics;`。

## 11. 开发约定（重要）

1. **TDD**：先写失败测试，再实现（铁律）。核心逻辑都在 `Services/` +
   几个接口后面，测试用 Fake 实现注入，**不要**在单测里真的杀进程。
2. 新增功能先看 `WatchdogServiceTests` / `MainViewModelTests` 的 Fake 模式，
   保持一致。
3. UI 改动请对照 `docs\superpowers\specs\dyknow-preview.html`（在另一目录
   `C:\Users\Administrator\Desktop\AI\Codex\Test2\docs\superpowers\specs\`），
   「UI 完全一致」是硬性要求。
4. 保持 0 警告 0 错误；`xUnit` 分析器警告也要消除。
5. 中文注释/中文 UI 文案可直接使用；代码标识符用英文。
6. 每次改动跑 `dotnet test`，green 后再提交；提交信息用中文描述改动。

## 12. 已知边界与未实现

- spec 3.4 的「Windows 服务 (sc create)」为可选，**未实现**（注册表自启动已够）。
- 杀进程用 `Process.Kill()`（单进程，非进程树）——若需连带子进程需改
  `ProcessKiller` 并补测试。
- 应用启动时若 `settings.AutoStartMonitoring` 为 true 且目录有效则自动开始监控
  （符合预览初始「运行中」状态）。
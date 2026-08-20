using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DyKnowWatchdog.Services;

/// <summary>Production implementation: kills a process via System.Diagnostics.Process.Kill().</summary>
public sealed class ProcessKiller : IProcessKiller
{
    public KillOutcome TryKill(ProcessSnapshot process)
    {
        try
        {
            using var p = Process.GetProcessById(process.Id);
            p.Kill();
            return KillOutcome.Killed;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5) // ERROR_ACCESS_DENIED
        {
            return KillOutcome.AccessDenied;
        }
        catch (InvalidOperationException)
        {
            return KillOutcome.AlreadyExited;
        }
        catch (ArgumentException)
        {
            return KillOutcome.AlreadyExited;
        }
        catch (NotSupportedException)
        {
            return KillOutcome.AccessDenied;
        }
    }
}
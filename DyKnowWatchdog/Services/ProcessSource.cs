using System.Diagnostics;

namespace DyKnowWatchdog.Services;

/// <summary>Production implementation: enumerates real running processes.</summary>
public sealed class ProcessSource : IProcessSource
{
    public IReadOnlyList<ProcessSnapshot> GetProcesses()
    {
        var result = new List<ProcessSnapshot>();
        Process[] procs;
        try
        {
            procs = Process.GetProcesses();
        }
        catch
        {
            return result;
        }

        foreach (var p in procs)
        {
            try
            {
                string? exe = null;
                try { exe = p.MainModule?.FileName; }
                catch { /* access denied / already exited */ }

                result.Add(new ProcessSnapshot(p.Id, p.ProcessName, exe));
            }
            catch
            {
                // process exited mid-enumeration — skip
            }
            finally
            {
                try { p.Dispose(); } catch { }
            }
        }
        return result;
    }
}
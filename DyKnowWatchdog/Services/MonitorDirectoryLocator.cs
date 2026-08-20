using System.Diagnostics;
using System.IO;

namespace DyKnowWatchdog.Services;

/// <summary>
/// Discovers the DyKnow monitor root directory.
/// <para>
/// Order:
/// 1. The preferred root (C:\Program Files\DyKnow\) if it exists.
/// 2. Otherwise locate a running DyKnow.exe process and walk up from its executable:
///    the deepest existing ancestor whose name contains "DyKnow" is the monitor root
///    (for the standard layout &lt;root&gt;\Client\DyKnow.exe this is &lt;root&gt; — the "父父目录",
///    i.e. the parent of DyKnow.exe's own directory). If no DyKnow-named ancestor exists,
///    fall back to the executable's own directory.
/// 3. Nothing found → null (UI shows an error message).
/// </para>
/// </summary>
public static class MonitorDirectoryLocator
{
    public static string? Locate(
        string? preferredRoot,
        Func<IEnumerable<string>>? dyKnowExePathProvider = null)
    {
        if (!string.IsNullOrWhiteSpace(preferredRoot) && Directory.Exists(preferredRoot))
            return Normalize(preferredRoot!);

        dyKnowExePathProvider ??= DefaultExePathProvider;

        foreach (var exePath in dyKnowExePathProvider())
        {
            if (string.IsNullOrWhiteSpace(exePath))
                continue;

            var exeDir = Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(exeDir) || !Directory.Exists(exeDir))
                continue;

            var found = FindDyKnowAncestor(exeDir);
            if (found != null)
                return Normalize(found);
        }

        return null;
    }

    private static string? FindDyKnowAncestor(string startingDir)
    {
        // Walk up from the exe directory to the drive root; the FIRST (deepest) ancestor
        // whose name contains "DyKnow" is the monitor root.
        var current = new DirectoryInfo(startingDir);
        while (current != null)
        {
            if (current.Name.Contains("DyKnow", StringComparison.OrdinalIgnoreCase))
                return current.FullName;
            current = current.Parent;
        }
        return null;
    }

    private static string Normalize(string path)
        => Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static IEnumerable<string> DefaultExePathProvider()
    {
        var result = new List<string>();
        foreach (var p in Process.GetProcessesByName("DyKnow"))
        {
            using (p)
            {
                try
                {
                    var fileName = p.MainModule?.FileName;
                    if (!string.IsNullOrWhiteSpace(fileName))
                        result.Add(fileName);
                }
                catch
                {
                    // process exited or access denied — skip
                }
            }
        }
        return result;
    }
}
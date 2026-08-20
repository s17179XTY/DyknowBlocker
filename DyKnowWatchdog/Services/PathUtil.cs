using System.IO;

namespace DyKnowWatchdog.Services;

public static class PathUtil
{
    /// <summary>
    /// Returns true when <paramref name="path"/> is inside <paramref name="root"/> (case-insensitive,
    /// separator-aware so a sibling like "C:\DyKnow2" is NOT considered inside "C:\DyKnow").
    /// </summary>
    public static bool IsWithinDirectory(string? root, string? path)
    {
        if (string.IsNullOrWhiteSpace(root) || string.IsNullOrWhiteSpace(path))
            return false;

        string r;
        string p;
        try
        {
            r = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
            p = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        }
        catch
        {
            return false;
        }

        if (string.Equals(r, p, StringComparison.OrdinalIgnoreCase))
            return true;

        return p.StartsWith(r + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }
}
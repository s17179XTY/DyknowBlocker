using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Tests;

public class MonitorDirectoryLocatorTests : IDisposable
{
    private readonly string _tempRoot;

    public MonitorDirectoryLocatorTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "DyKnowLoc_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempRoot);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_tempRoot)) Directory.Delete(_tempRoot, true); }
        catch { /* best effort */ }
    }

    private string CreateDir(params string[] segments)
    {
        var path = Path.Combine(new[] { _tempRoot }.Concat(segments).ToArray());
        Directory.CreateDirectory(path);
        return path;
    }

    private string CreateFile(params string[] segments)
    {
        var path = Path.Combine(new[] { _tempRoot }.Concat(segments).ToArray());
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "");
        return path;
    }

    [Fact]
    public void Locate_WhenPreferredRootExists_ReturnsItNormalized()
    {
        var root = CreateDir("DyKnow");
        Directory.CreateDirectory(Path.Combine(root, "Client"));

        var result = MonitorDirectoryLocator.Locate(root, () => Array.Empty<string>());

        Assert.Equal(Normalize(root), result);
    }

    [Fact]
    public void Locate_WhenPreferredMissing_FindsParentParentOfExeDirectory()
    {
        // DyKnow.exe at <root>\Client\DyKnow.exe → monitored root = <root> (the "父父目录")
        var exePath = CreateFile("DyKnow", "Client", "DyKnow.exe");

        var result = MonitorDirectoryLocator.Locate("Z:\\NoSuchDir_" + Guid.NewGuid().ToString("N"),
            () => new[] { exePath });

        Assert.Equal(Normalize(Path.Combine(_tempRoot, "DyKnow")), result);
    }

    [Fact]
    public void Locate_WhenExeHasNoParentDirectory_UsesExeDirectory()
    {
        // DyKnow.exe at <root>\DyKnow.exe → monitored root = <root> (falls back to exe's own dir)
        var exePath = CreateFile("DyKnowFlat", "DyKnow.exe");

        var result = MonitorDirectoryLocator.Locate("Z:\\NoSuchDir_" + Guid.NewGuid().ToString("N"),
            () => new[] { exePath });

        Assert.Equal(Normalize(Path.Combine(_tempRoot, "DyKnowFlat")), result);
    }

    [Fact]
    public void Locate_WhenExePathDoesNotExist_SkipsIt()
    {
        var missing = Path.Combine(_tempRoot, "DyKnow", "Client", "DyKnow.exe");

        var result = MonitorDirectoryLocator.Locate("Z:\\NoSuchDir_" + Guid.NewGuid().ToString("N"),
            () => new[] { missing });

        Assert.Null(result);
    }

    [Fact]
    public void Locate_WhenNoExePaths_ReturnsNull()
    {
        var result = MonitorDirectoryLocator.Locate("Z:\\NoSuchDir_" + Guid.NewGuid().ToString("N"),
            () => Array.Empty<string>());

        Assert.Null(result);
    }

    [Fact]
    public void Locate_WhenExePathsContainNull_ReturnsNull()
    {
        var result = MonitorDirectoryLocator.Locate("Z:\\NoSuchDir_" + Guid.NewGuid().ToString("N"),
            () => new string?[] { null! }.Select(p => p!));

        Assert.Null(result);
    }

    private static string Normalize(string path)
        => Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
}
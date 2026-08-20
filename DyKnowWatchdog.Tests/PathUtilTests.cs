using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Tests;

public class PathUtilTests
{
    [Theory]
    [InlineData(@"C:\DyKnow\Client\chrome.exe", @"C:\DyKnow")]
    [InlineData(@"C:\DyKnow\Client\sub\app.exe", @"C:\DyKnow")]
    [InlineData(@"C:\DyKnow\DyKnow.exe", @"C:\DyKnow")]
    [InlineData(@"C:\DYKNOW\client\App.exe", @"c:\dyknow")] // case-insensitive
    [InlineData(@"C:\Program Files\DyKnow\Cloud\svc.exe", @"C:\Program Files\DyKnow")]
    public void IsWithinDirectory_ReturnsTrue(string path, string root)
    {
        Assert.True(PathUtil.IsWithinDirectory(root, path));
    }

    [Theory]
    [InlineData(@"C:\DyKnow2\Client\chrome.exe", @"C:\DyKnow")]      // sibling name prefix
    [InlineData(@"C:\Other\chrome.exe", @"C:\DyKnow")]
    [InlineData(@"D:\DyKnow\Client\chrome.exe", @"C:\DyKnow")]       // different drive
    [InlineData(@"C:\DyKnow", @"C:\DyKnow\Client")]                  // reversed
    public void IsWithinDirectory_ReturnsFalse(string path, string root)
    {
        Assert.False(PathUtil.IsWithinDirectory(root, path));
    }

    [Fact]
    public void IsWithinDirectory_NullOrEmpty_ReturnsFalse()
    {
        Assert.False(PathUtil.IsWithinDirectory(null!, @"C:\DyKnow\Client\chrome.exe"));
        Assert.False(PathUtil.IsWithinDirectory(@"C:\DyKnow", null!));
        Assert.False(PathUtil.IsWithinDirectory("", ""));
    }

    [Fact]
    public void IsWithinDirectory_RelativeVsAbsolute_Normalizes()
    {
        // absolute path under root with different separator style / trailing slash
        Assert.True(PathUtil.IsWithinDirectory(@"C:\DyKnow\", @"C:\DyKnow\Client\app.exe"));
    }
}
using DyKnowWatchdog.Services;

namespace DyKnowWatchdog.Tests;

public class FrequencyParserTests
{
    [Theory]
    [InlineData("0.5", 0.5)]
    [InlineData("0.5s", 0.5)]
    [InlineData("2", 2.0)]
    [InlineData("120", 120.0)]
    [InlineData("0.1", 0.1)]
    [InlineData("3600", 3600.0)]
    [InlineData("1.5", 1.5)]
    [InlineData(" 3 ", 3.0)]
    [InlineData("3s", 3.0)]
    public void TryParse_ValidValues_ReturnsSeconds(string input, double expected)
    {
        var ok = FrequencyParser.TryParse(input, out var seconds);

        Assert.True(ok);
        Assert.Equal(expected, seconds, 3);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("0.05")]
    [InlineData("3601")]
    [InlineData("-1")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("1.2.3")]
    [InlineData("1e3")]
    [InlineData("5m")]
    [InlineData(null)]
    public void TryParse_InvalidValues_ReturnsFalse(string? input)
    {
        var ok = FrequencyParser.TryParse(input, out _);

        Assert.False(ok);
    }

    [Theory]
    [InlineData(0.5, "0.5")]
    [InlineData(2.0, "2")]
    [InlineData(0.1, "0.1")]
    [InlineData(3600.0, "3600")]
    [InlineData(1.5, "1.5")]
    public void Format_RemovesTrailingZeros(double seconds, string expected)
    {
        Assert.Equal(expected, FrequencyParser.Format(seconds));
    }
}
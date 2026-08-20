using System.Globalization;

namespace DyKnowWatchdog.Services;

public static class FrequencyParser
{
    public const double MinSeconds = 0.1;
    public const double MaxSeconds = 3600.0;
    public const double DefaultSeconds = 0.5;

    /// <summary>
    /// Parses a monitoring frequency in seconds, e.g. "0.5", "0.5s", "2", "120".
    /// Accepts an optional trailing 's' / 'S'. Rejects exponents, multiple dots,
    /// out-of-range values and garbage. Range: [0.1, 3600].
    /// </summary>
    public static bool TryParse(string? input, out double seconds)
    {
        seconds = DefaultSeconds;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var s = input.Trim();
        if (s.EndsWith('s') || s.EndsWith('S'))
            s = s[..^1].Trim();

        if (!double.TryParse(s,
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var value))
        {
            return false;
        }

        if (value < MinSeconds || value > MaxSeconds)
            return false;

        seconds = value;
        return true;
    }

    /// <summary>Formats seconds without trailing zeros: 0.5 → "0.5", 2 → "2", 3600 → "3600".</summary>
    public static string Format(double seconds)
        => seconds.ToString("0.####", CultureInfo.InvariantCulture);
}
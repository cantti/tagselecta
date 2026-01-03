using System.Text.RegularExpressions;

namespace TagSelecta.Shared;

public static class StringListExtensions
{
    // todo custom separator in settings
    public static string ToJoined(this IEnumerable<string?> strings)
    {
        return string.Join("; ", strings);
    }

    public static List<string> ToMulti(this string str)
    {
        return str.Split(";").Select(x => x.Trim()).ToList();
    }

    public static string NormalizeKey(this string? value)
    {
        return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    public static string ToSpacedWords(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }

    public static string SubstringFromEnd(this string value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
            return string.Empty;

        return value.Length <= length ? value : value.Substring(value.Length - length, length);
    }
}

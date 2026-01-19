using System.Text.RegularExpressions;

namespace TagSelecta.Shared.Tagging;

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

    public static string DirectoryName(this string path)
    {
        return Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException(
                "Unable to determine the directory name for the provided path."
            );
    }
}

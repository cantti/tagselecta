using System.Text.RegularExpressions;
using TagSelecta.Shared.IO;

namespace TagSelecta.Shared.Tagging;

public static class StringListExtensions
{
    private static readonly string[] _multiValueFields =
    [
        FieldName.AlbumArtist,
        FieldName.Artist,
        FieldName.Composer,
        FieldName.Genre,
    ];

    public static string JoinTagValues(this IEnumerable<string?> strings)
    {
        return string.Join("; ", strings.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public static List<string> SplitTagValues(this string str)
    {
        return str.Split(";").Select(x => x.Trim()).ToList();
    }

    public static List<string> SplitTagValuesIfNeeded(this string str, string key)
    {
        return _multiValueFields.Contains(key) ? str.SplitTagValues() : [str];
    }

    public static string NormalizeKey(this string? value)
    {
        return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    public static string ToSpacedWords(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }

    public static string SubstringFromEnd(this string value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
        {
            return string.Empty;
        }

        return value.Length <= length ? value : value.Substring(value.Length - length, length);
    }

    public static string DirectoryName(this string path)
    {
        return PathUtils.GetDirectoryName(path)
            ?? throw new InvalidOperationException(
                "Unable to determine the directory name for the provided path."
            );
    }
}

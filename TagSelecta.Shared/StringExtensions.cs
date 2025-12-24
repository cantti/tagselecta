using System.Text.RegularExpressions;

namespace TagSelecta.Shared;

public static class StringListExtensions
{
    // todo custom separator in settings
    public static string ToJoined(this IEnumerable<string> strings)
    {
        return string.Join("; ", strings);
    }

    public static List<string> ToMulti(this string str)
    {
        return str.Split(";").Select(x => x.Trim()).ToList();
    }

    public static string ToSpacedWords(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }
}

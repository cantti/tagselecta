using System.Text.RegularExpressions;

namespace SongTagCli.Misc;

public static class StringListExtensions
{
    public static string Print(this IEnumerable<string> strings)
    {
        return string.Join("; ", strings);
    }

    public static void Debug(this string? value)
    {
        if (value is null)
            Console.WriteLine("Value is null");
        else if (value == "")
            Console.WriteLine("Value is empty");
        else if (string.IsNullOrWhiteSpace(value))
            Console.WriteLine("Value is whitespace only");
        else
            Console.WriteLine($"Value: \"{value}\" (Length: {value.Length})");
    }

    public static string CleanFileName(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input
            .Replace(Path.DirectorySeparatorChar.ToString(), "")
            .Replace(Path.AltDirectorySeparatorChar.ToString(), "");
        input = Regex.Replace(input, @"\s+", " ");
        return input;
    }
}

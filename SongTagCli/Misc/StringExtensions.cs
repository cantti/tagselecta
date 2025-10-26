namespace SongTagCli.Misc;

public static class StringListExtensions
{
    public static string Print(this IEnumerable<string> strings)
    {
        return string.Join("; ", strings);
    }
}

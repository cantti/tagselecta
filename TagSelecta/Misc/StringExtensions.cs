namespace TagSelecta.Misc;

public static class StringListExtensions
{
    // todo custom separator in settings
    public static string Print(this IEnumerable<string> strings)
    {
        return string.Join("; ", strings);
    }
}

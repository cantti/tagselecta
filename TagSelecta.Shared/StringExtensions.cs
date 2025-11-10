namespace TagSelecta.Shared;

public static class StringListExtensions
{
    // todo custom separator in settings
    public static string Joined(this IEnumerable<string> strings)
    {
        return string.Join("; ", strings);
    }
}

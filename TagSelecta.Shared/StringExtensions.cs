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
}

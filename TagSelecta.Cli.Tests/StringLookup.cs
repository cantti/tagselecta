namespace TagSelecta.Cli.Tests;

public static class StringLookup
{
    public static ILookup<string, string?> Empty()
    {
        return Enumerable.Empty<(string Key, string? Value)>().ToLookup(x => x.Key, x => x.Value);
    }
}

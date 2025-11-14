using System.Reflection;

namespace TagSelecta.Tagging;

public static class TagDataFieldNames
{
    public const string AlbumArtist = "albumartist";
    public const string Artist = "artist";
    public const string Album = "album";
    public const string Year = "year";
    public const string Title = "title";
    public const string Track = "track";
    public const string TrackTotal = "tracktotal";
    public const string Disc = "disc";
    public const string DiscTotal = "disctotal";
    public const string Genre = "genre";
    public const string Comment = "comment";
    public const string Composer = "composer";
    public const string Label = "label";
    public const string CatalogNumber = "catalognumber";
    public const string Pictures = "pictures";

    public static IReadOnlyList<string> All { get; } =
        typeof(TagDataFieldNames)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToArray();
}

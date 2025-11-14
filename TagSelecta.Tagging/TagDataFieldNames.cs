using System.Reflection;

namespace TagSelecta.Tagging;

public static class TagDataFieldNames
{
    public const string AlbumArtist = nameof(TagData.AlbumArtist);
    public const string Artist = nameof(TagData.Artist);
    public const string Album = nameof(TagData.Album);
    public const string Year = nameof(TagData.Year);
    public const string Title = nameof(TagData.Title);
    public const string Track = nameof(TagData.Track);
    public const string TrackTotal = nameof(TagData.TrackTotal);
    public const string Disc = nameof(TagData.Disc);
    public const string DiscTotal = nameof(TagData.DiscTotal);
    public const string Genre = nameof(TagData.Genre);
    public const string Comment = nameof(TagData.Comment);
    public const string Composer = nameof(TagData.Composer);
    public const string Label = nameof(TagData.Label);
    public const string CatalogNumber = nameof(TagData.CatalogNumber);
    public const string Pictures = nameof(TagData.Pictures);

    public static IReadOnlyList<string> All { get; } =
        typeof(TagDataFieldNames)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToArray();
}

namespace TagSelecta.App.Config;

public class DiscogsSection
{
    public Dictionary<string, string> FieldMap { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["album"] = "$.title",
            ["date"] = "$.year",
            ["label"] = "$.labels[0].name",
            ["catalognumber"] = "$.labels[0].catno",
            ["genre"] = "$.styles[*]",
            ["albumartist"] = "auto",
            ["artist"] = "auto",
            ["title"] = "$.tracklist[?(@.type_=='track')].title",
            ["track"] = "auto",
            ["discogs_release_id"] = "$.id",
        };

    public List<string> PerTrackFields { get; set; } = ["artist", "title", "track"];
}

namespace TagSelecta.App.Config;

public class MusicBrainzSection
{
    public Dictionary<string, string> FieldMap { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["album"] = "$.title",
            ["albumartist"] = "$['artist-credit'][0].artist.name",
            ["artist"] = "$['artist-credit'][0].artist.name",
            ["title"] = "$.media[*].tracks[*].title",
            ["track"] = "auto",
            ["tracktotal"] = "auto",
            ["genre"] = "$['release-group'].genres[*].name",
            ["date"] = "$.date",
            ["label"] = "$['label-info'][0].label.name",
            ["catalognumber"] = "$['label-info'][0]['catalog-number']",
            ["comment"] = "$.disambiguation",
            ["musicbrainz_release_id"] = "$.id",
        };

    public List<string> PerTrackFields { get; set; } = ["title", "track"];
}

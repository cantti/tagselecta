namespace TagSelecta.App.Config;

public class MusicBrainzSection
{
    public Dictionary<string, string> FieldMap { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["album"] = "{{ release.title }}",
            ["albumartist"] = "{{ release.artistcredit[0].name }}",
            ["artist"] =
                "{{ if tracks[index].artistcredit && tracks[index].artistcredit.size > 0; tracks[index].artistcredit | array.map 'name' | joined; else; release.artistcredit | array.map 'name' | joined; end }}",
            ["catalognumber"] = "{{ release.labelinfo[0].catalognumber }}",
            ["comment"] = "{{ release.disambiguation }}",
            ["date"] = "{{ release.date }}",
            ["genre"] = "{{ release.releasegroup.genres | array.map 'name' | joined }}",
            ["label"] = "{{ release.labelinfo[0].label.name }}",
            ["title"] = "{{ tracks[index].title }}",
            ["track"] = "{{ index + 1 }}",
            ["tracktotal"] = "{{ tracks.size }}",
            ["musicbrainz_release_id"] = "{{ release.id }}",
        };
}

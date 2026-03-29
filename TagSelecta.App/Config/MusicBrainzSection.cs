namespace TagSelecta.App.Config;

public class MusicBrainzSection
{
    public Dictionary<string, string> FieldMap { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["album"] = "{{ release.title }}",
            ["albumartist"] = "{{ release.artistcredit | array.map 'name' | joined }}",
            ["artist"] =
                "{{ if tracks[index].artistcredit.size > 0; tracks[index].artistcredit | array.map 'name' | joined; else; release.artistcredit | array.map 'name' | joined; end }}",
            ["catalognumber"] =
                "{{ release.labelinfo | array.map 'label' | array.map 'catalognumber' | joined }}",
            ["comment"] = "{{ release.disambiguation }}",
            ["date"] = "{{ release.date }}",
            ["genre"] = "{{ release.releasegroup?.genres | array.map 'name' | joined }}",
            ["label"] = "{{ release.labelinfo | array.map 'label' | array.map 'name' | joined }}",
            ["title"] = "{{ tracks[index]?.title }}",
            ["track"] = "{{ index + 1 }}",
            ["tracktotal"] = "{{ tracks.size }}",
            ["musicbrainz_release_id"] = "{{ release.id }}",
        };
}

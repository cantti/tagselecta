namespace TagSelecta.App.Config;

public class DiscogsSection
{
    public Dictionary<string, string> FieldMap { get; set; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["album"] = "{{ release.title }}",
            ["date"] = "{{ release.year }}",
            ["label"] = "{{ release.labels[0].name }}",
            ["catalognumber"] = "{{ release.labels[0].catno }}",
            ["genre"] = "{{ release.styles | joined }}",
            ["albumartist"] = "{{ release.artists | array.map 'name' | joined }}",
            ["artist"] =
                "{{ if tracks[index].artists && tracks[index].artists.size > 0; tracks[index].artists | array.map 'name' | joined; else; release.artists | array.map 'name' | joined; end }}",
            ["title"] = "{{ tracks[index].title }}",
            ["track"] = "{{ index + 1 }}",
            ["tracktotal"] = "{{ tracks.size }}",
            ["discogs_release_id"] = "{{ release.id }}",
        };
}

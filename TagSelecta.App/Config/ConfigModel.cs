using TagSelecta.Commands.Tui;

namespace TagSelecta.App.Config;

public class ConfigModel
{
    public GeneralSection General { get; set; } = new();
    public Dictionary<string, string> Macros { get; set; } = new();
    public DiscogsSection Discogs { get; set; } = new();
    public MusicBrainzSection MusicBrainz { get; set; } = new();
    public TuiAppTheme Theme { get; set; } = new();
    public Dictionary<string, string> Icons { get; set; } =
        new()
        {
            ["path"] = "\uf07b",
            ["artist"] = "\uf007",
            ["albumartist"] = "\uf0c0",
            ["album"] = "\uf51f",
            ["bpm"] = "\uf21e",
            ["comment"] = "\uf075",
            ["composer"] = "\uf040",
            ["conductor"] = "\uf007",
            ["copyright"] = "\uf1f9",
            ["discnumber"] = "\uf51f",
            ["disctotal"] = "\uf51f",
            ["isrc"] = "\uf02a",
            ["publisher"] = "\uf1ad",
            ["title"] = "\uf001",
            ["tracknumber"] = "\uf292",
            ["tracktotal"] = "\uf0ca",
            ["date"] = "\uf073",
            ["genre"] = "\uf02b",
            ["picture"] = "\uf03e",
        };
}

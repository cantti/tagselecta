using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.Edit;

public class EditSettings : TagDataActionSettings
{
    [CommandOption($"--{Fields.Album}|-l")]
    [Description("Album name.")]
    public string? Album { get; set; }

    [CommandOption($"--{Fields.AlbumArtist}|-A")]
    [Description(
        "One or more album artists. Multiple values can be provided using a ';' separator."
    )]
    public string? AlbumArtist { get; set; }

    [CommandOption($"--{Fields.Artist}|-a")]
    [Description("One or more artists. Multiple values can be provided using a ';' separator")]
    public string? Artist { get; set; }

    [CommandOption($"--{Fields.Bpm}")]
    [Description("Beat per minutes")]
    public string? Bpm { get; set; }

    [CommandOption($"--{Fields.CatalogNumber}")]
    [Description("Catalog number.")]
    public string? CatalogNumber { get; set; }

    [CommandOption($"--{Fields.Comment}|-c")]
    [Description("Comment or notes.")]
    public string? Comment { get; set; }

    [CommandOption($"--{Fields.Composer}|-C")]
    [Description("Composer.")]
    public string? Composer { get; set; }

    [CommandOption($"--{Fields.Conductor}")]
    [Description("Conductor.")]
    public string? Conductor { get; set; }

    [CommandOption($"--{Fields.Copyright}")]
    [Description("Copyright.")]
    public string? Copyright { get; set; }

    [CommandOption($"--{Fields.Date}|-y")]
    [Description("Release date.")]
    public string? Date { get; set; }

    [CommandOption($"--{Fields.Disc}|-d")]
    [Description("Disc number.")]
    public string? Disc { get; set; }

    [CommandOption($"--{Fields.DiscTotal}|-D")]
    [Description("Total number of discs.")]
    public string? DiscTotal { get; set; }

    [CommandOption($"--{Fields.DiscogsReleaseId}")]
    [Description("Discogs release id.")]
    public string? DiscogsReleaseId { get; set; }

    [CommandOption($"--{Fields.Genre}|-g")]
    [Description("One or more genres. Multiple values can be provided using a ';' separator")]
    public string? Genre { get; set; }

    [CommandOption($"--{Fields.Isrc}")]
    [Description("International standard recording code")]
    public string? Isrc { get; set; }

    [CommandOption($"--{Fields.Label}")]
    [Description("Record label.")]
    public string? Label { get; set; }

    [CommandOption($"--{Fields.Publisher}")]
    [Description("Publisher.")]
    public string? Publisher { get; set; }

    [CommandOption($"--{Fields.Title}|-t")]
    [Description("Track title.")]
    public string? Title { get; set; }

    [CommandOption($"--{Fields.Track}|-n")]
    [Description("Track number.")]
    public string? Track { get; set; }

    [CommandOption($"--{Fields.TrackTotal}|-N")]
    [Description("Total number of tracks.")]
    public string? TrackTotal { get; set; }

    [CommandOption($"--{Fields.Set}|-s")]
    [Description(
        "Custom fields in key=value format. Use this option multiple times to include multiple fields (e.g., -c key1=value1 -c key2=value2)."
    )]
    public string[]? Set { get; set; }

    [CommandOption("--clearcustom")]
    [Description("Clear all other custom fields.")]
    public bool ClearCustom { get; set; }

    [CommandOption($"--{Fields.Picture}|-p")]
    [Description(
        "Path to a picture file. Use this option multiple times to include multiple images (e.g., -p path1 -p path2)."
    )]
    public string[]? Picture { get; set; }

    [CommandOption($"--{Fields.PictureType}")]
    [Description(
        "Type of each picture provided. Specify multiple times to match the order of the pictures. This option is optional.\nCommon values: FrontCover, BackCover, Artist, Other."
    )]
    public string[]? PictureType { get; set; }

    [CommandOption("--clearpicture")]
    [Description("Clear all other pictures")]
    public bool ClearPicture { get; set; }

    private static string NormalizeKey(string key) =>
        key switch
        {
            "l" => Fields.Album,
            "A" => Fields.AlbumArtist,
            "a" => Fields.Artist,
            "c" => Fields.Comment,
            "y" => Fields.Date,
            "d" => Fields.Disc,
            "D" => Fields.DiscTotal,
            "g" => Fields.Genre,
            "t" => Fields.Title,
            "n" => Fields.Track,
            "N" => Fields.TrackTotal,
            _ => key,
        };

    public override void ParseTuiArgs(IEnumerable<TagDataActionArg> args)
    {
        var argLookup = args.GroupBy(a => NormalizeKey(a.Key))
            .ToDictionary(g => g.Key, g => g.ToList());

        string? TakeSingle(string key)
        {
            if (!argLookup.TryGetValue(key, out var list) || list.Count == 0)
            {
                return null;
            }

            var value = list[0].Value;
            list.RemoveAt(0);

            if (list.Count == 0)
            {
                argLookup.Remove(key);
            }

            return value;
        }

        string[]? TakeAll(string key)
        {
            if (!argLookup.Remove(key, out var list))
            {
                return null;
            }

            return list.Select(x => x.Value).ToArray();
        }

        bool TakeAny(Func<TagDataActionArg, bool> predicate)
        {
            bool found = false;

            foreach (var kvp in argLookup.ToList())
            {
                var matched = kvp.Value.Where(predicate).ToList();
                if (matched.Count == 0)
                    continue;

                found = true;
                foreach (var m in matched)
                    kvp.Value.Remove(m);

                if (kvp.Value.Count == 0)
                    argLookup.Remove(kvp.Key);
            }

            return found;
        }

        // 3. Parse known arguments

        Album = TakeSingle(Fields.Album);
        AlbumArtist = TakeSingle(Fields.AlbumArtist);
        Artist = TakeSingle(Fields.Artist);
        Bpm = TakeSingle(Fields.Bpm);
        CatalogNumber = TakeSingle(Fields.CatalogNumber);
        Comment = TakeSingle(Fields.Comment);
        Composer = TakeSingle(Fields.Composer);
        Conductor = TakeSingle(Fields.Conductor);
        Copyright = TakeSingle(Fields.Copyright);
        Date = TakeSingle(Fields.Date);
        Disc = TakeSingle(Fields.Disc);
        DiscTotal = TakeSingle(Fields.DiscTotal);
        DiscogsReleaseId = TakeSingle(Fields.DiscogsReleaseId);
        Genre = TakeSingle(Fields.Genre);
        Isrc = TakeSingle(Fields.Isrc);
        Label = TakeSingle(Fields.Label);
        Publisher = TakeSingle(Fields.Publisher);
        Title = TakeSingle(Fields.Title);
        Track = TakeSingle(Fields.Track);
        TrackTotal = TakeSingle(Fields.TrackTotal);

        Set = TakeAll("set");

        ClearCustom = TakeSingle("clearcustom") == "true";

        var unmappedArgs = argLookup.SelectMany(kvp => kvp.Value).ToList();

        if (unmappedArgs.Count > 0)
        {
            throw new InvalidOperationException("Unknown arguments");
        }
    }
}

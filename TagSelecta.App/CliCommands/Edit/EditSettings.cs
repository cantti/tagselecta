using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.App.Tui.TuiCommands;

namespace TagSelecta.App.CliCommands.Edit;

public class EditSettings : BaseSettings
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

    public override void ParseTuiArgs(IEnumerable<Arg> args)
    {
        Album = args.FirstOrDefault(x => x.Key == Fields.Album || x.Key == "l")?.Value;
        AlbumArtist = args.FirstOrDefault(x => x.Key == Fields.AlbumArtist || x.Key == "A")?.Value;
        Artist = args.FirstOrDefault(x => x.Key == Fields.Artist || x.Key == "a")?.Value;
        Bpm = args.FirstOrDefault(x => x.Key == Fields.Bpm)?.Value;
        CatalogNumber = args.FirstOrDefault(x => x.Key == Fields.CatalogNumber)?.Value;
        Comment = args.FirstOrDefault(x => x.Key == Fields.Comment || x.Key == "c")?.Value;
        Composer = args.FirstOrDefault(x => x.Key == Fields.Composer || x.Key == "C")?.Value;
        Conductor = args.FirstOrDefault(x => x.Key == Fields.Conductor)?.Value;
        Copyright = args.FirstOrDefault(x => x.Key == Fields.Copyright)?.Value;
        Date = args.FirstOrDefault(x => x.Key == Fields.Date || x.Key == "y")?.Value;
        Disc = args.FirstOrDefault(x => x.Key == Fields.Disc || x.Key == "d")?.Value;
        DiscTotal = args.FirstOrDefault(x => x.Key == Fields.DiscTotal || x.Key == "D")?.Value;
        DiscogsReleaseId = args.FirstOrDefault(x => x.Key == Fields.DiscogsReleaseId)?.Value;
        Genre = args.FirstOrDefault(x => x.Key == Fields.Genre || x.Key == "g")?.Value;
        Isrc = args.FirstOrDefault(x => x.Key == Fields.Isrc)?.Value;
        Label = args.FirstOrDefault(x => x.Key == Fields.Label)?.Value;
        Publisher = args.FirstOrDefault(x => x.Key == Fields.Publisher)?.Value;
        Title = args.FirstOrDefault(x => x.Key == Fields.Title || x.Key == "t")?.Value;
        Track = args.FirstOrDefault(x => x.Key == Fields.Track || x.Key == "n")?.Value;
        TrackTotal = args.FirstOrDefault(x => x.Key == Fields.TrackTotal || x.Key == "N")?.Value;
        Set = args.Where(x => x.Key == "set").Select(x => x.Value).ToArray();
        ClearCustom = args.Any(x => x.Key == "clearcustom");
        // todo
        // Picture =
        // PictureType =
        // ClearPicture =
    }
}

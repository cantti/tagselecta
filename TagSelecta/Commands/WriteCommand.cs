using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Formatting;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class WriteSettings : FileSettings
{
    [CommandOption("--genre|-g")]
    public string[]? Genre { get; set; }

    [CommandOption("--artist|-a")]
    public string[]? Artist { get; set; }

    [CommandOption("--albumartist|-A")]
    public string[]? AlbumArtist { get; set; }

    [CommandOption("--title|-t")]
    public string? Title { get; set; }

    [CommandOption("--subtitle")]
    public string? Subtitle { get; set; }

    [CommandOption("--album|-l")]
    public string? Album { get; set; }

    [CommandOption("--year|-y")]
    public uint? Year { get; set; }

    [CommandOption("--track|-n")]
    public uint? Track { get; set; }

    [CommandOption("--tracktotal|-N")]
    public uint? TrackTotal { get; set; }

    [CommandOption("--comment|-c")]
    public string? Comment { get; set; }

    [CommandOption("--disc|-d")]
    public uint? Disc { get; set; }

    [CommandOption("--disctotal|-D")]
    public uint? DiscTotal { get; set; }

    [CommandOption("--label|-L")]
    public string? Label { get; set; }

    [CommandOption("--catalogno|-C")]
    public string? CatalogNumber { get; set; }

    [CommandOption("--bpm")]
    public uint? Bpm { get; set; }

    [CommandOption("--description")]
    public string? Description { get; set; }

    [CommandOption("--composers")]
    public string[]? Composers { get; set; }

    [CommandOption("--conductor")]
    public string? Conductor { get; set; }

    [CommandOption("--isrc")]
    public string? Isrc { get; set; }

    [CommandOption("--lyrics")]
    public string? Lyrics { get; set; }

    [CommandOption("--publisher")]
    public string? Publisher { get; set; }

    [CommandOption("--copyright")]
    public string? Copyright { get; set; }
}

public class WriteCommand(IAnsiConsole console) : FileCommand<WriteSettings>(console)
{
    protected override void BeforeExecute()
    {
        // convert arrays with empty first element to empty arrays
        foreach (var prop in typeof(WriteSettings).GetProperties())
        {
            if (prop.Name == nameof(WriteSettings.Path))
                continue;
            var val = prop.GetValue(Settings);
            if (val is null)
                continue;
            if (val is string[] valArray)
            {
                if (valArray.First() == "")
                {
                    prop.SetValue(Settings, Array.Empty<string>());
                }
            }
        }
    }

    protected override void Execute()
    {
        var mapper = new WriteSettingsMapper(TagData.Clone());
        mapper.Map(Settings, TagData);
        WriteTags();
    }
}

[Mapper(
    PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive,
    // it should work fine even without it
    // https://mapperly.riok.app/docs/configuration/mapper/#null-values
    AllowNullPropertyAssignment = false
)]
public partial class WriteSettingsMapper(TagData originalTags)
{
    [SuppressMessage("Mapper", "RMG089")]
    [SuppressMessage("Mapper", "RMG090")]
    [MapProperty(nameof(WriteSettings.Artist), nameof(TagData.Artists))]
    [MapProperty(nameof(WriteSettings.Genre), nameof(TagData.Genres))]
    [MapProperty(nameof(WriteSettings.AlbumArtist), nameof(TagData.AlbumArtists))]
    [MapperIgnoreSource(nameof(WriteSettings.Path))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzArtistId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzArtistId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseArtistId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseArtistId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzArtistId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzDiscId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseGroupId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseArtistId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzTrackId))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseStatus))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseType))]
    [MapperIgnoreTarget(nameof(TagData.MusicBrainzReleaseCountry))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainTrackGain))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainTrackPeak))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainAlbumGain))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainAlbumPeak))]
    [MapperIgnoreTarget(nameof(TagData.Pictures))]
    [MapperIgnoreTarget(nameof(TagData.DiscogsReleaseId))]
    [MapperIgnoreTarget(nameof(TagData.Path))]
    public partial void Map(WriteSettings settings, TagData tagData);

    [SuppressMessage("Mapper", "IDE0051")]
    private string MapString(string val) => Formatter.Format(val, originalTags);

    [SuppressMessage("Mapper", "IDE0051")]
    private List<string> MapList(string[] array)
    {
        var result = new List<string>();
        foreach (var val in array)
        {
            var newVal = Formatter.Format(val, originalTags);
            result.AddRange(newVal.Split(';').Select(x => x.Trim()));
        }
        return result;
    }
}

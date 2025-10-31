using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

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

public class WriteAction : FileAction<WriteSettings>
{
    public override void Execute(ActionContext<WriteSettings> context)
    {
        // todo find better way
        // reflection?
        context.Settings.Artist =
            context.Settings.Artist?.First() == "" ? null : context.Settings.Artist;
        context.Settings.AlbumArtist =
            context.Settings.AlbumArtist?.First() == "" ? null : context.Settings.AlbumArtist;
        context.Settings.Genre =
            context.Settings.Genre?.First() == "" ? null : context.Settings.Genre;
        context.Settings.Composers =
            context.Settings.Composers?.First() == "" ? null : context.Settings.Composers;

        var mapper = new WriteSettingsMapper();

        var tags = Tagger.ReadTags(context.File);

        mapper.FromSettings(context.Settings, tags);

        context.Console.PrintTagData(tags);

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
            return;
        }
    }
}

[Mapper(
    PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive,
    // it should work fine even without it
    // https://mapperly.riok.app/docs/configuration/mapper/#null-values
    AllowNullPropertyAssignment = false
)]
public partial class WriteSettingsMapper
{
    [SuppressMessage("Mapper", "RMG089")]
    [SuppressMessage("Mapper", "RMG090")]
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
    [MapperIgnoreTarget(nameof(TagData.MusicIpId))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainTrackGain))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainTrackPeak))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainAlbumGain))]
    [MapperIgnoreTarget(nameof(TagData.ReplayGainAlbumPeak))]
    [MapperIgnoreTarget(nameof(TagData.Pictures))]
    [MapperIgnoreTarget(nameof(TagData.AmazonId))]
    public partial void FromSettings(WriteSettings settings, TagData tagData);
}

using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.MusicBrainz;

public class MusicBrainzSettings : TagDataActionSettings
{
    [CommandOption("--release|-r", true)]
    [Description("MusicBrainz release URL or release id")]
    public required string Release { get; set; }
}

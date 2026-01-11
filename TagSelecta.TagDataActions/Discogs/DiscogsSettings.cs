using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.Discogs;

public class DiscogsSettings : TagDataActionSettings
{
    [CommandOption("--release|-r")]
    public string Release { get; set; } = "";

    [CommandOption("--fields|-f")]
    [Description(
        "Fields to update from Discogs release. If not specified, all values will be updated"
    )]
    public string? Fields { get; set; }

    public override void ParseTuiArgs(IEnumerable<TagDataActionArg> args) { }
}

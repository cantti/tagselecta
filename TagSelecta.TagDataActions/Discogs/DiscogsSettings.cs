using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Discogs;

public class DiscogsSettings : TagDataActionSettings
{
    [CommandOption("--url|-u", isRequired: true)]
    [Description("Discogs release URL")]
    public required string Url { get; set; }

    [CommandOption("--fields|-f")]
    [Description(
        "Fields to update from Discogs release. If not specified, all values will be updated"
    )]
    public string? Fields { get; set; }
}

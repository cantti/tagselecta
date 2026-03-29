using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Discogs;

public class DiscogsSettings : TagDataActionSettings
{
    [CommandOption("--url|-u", true)]
    [Description("Discogs release URL")]
    public required string Url { get; set; }
}

using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Discogs;

public class DiscogsSettings : TagDataActionSettings
{
    [CommandOption("--release|-r", true)]
    [Description("Discogs release or master URL")]
    public required string Release { get; set; }
}

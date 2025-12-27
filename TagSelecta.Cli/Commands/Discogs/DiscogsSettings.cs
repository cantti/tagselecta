using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.Discogs;

public class DiscogsSettings : BaseSettings
{
    [CommandOption("--release|-r")]
    public string Release { get; set; } = "";

    [CommandOption("--fields|-f")]
    [Description(
        "Fields to update from Discogs release. If not specified, all values will be updated"
    )]
    public string? Fields { get; set; }
}
using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Cli.Find;

public class FindSettings : TagDataActionSettings
{
    [CommandOption("--query|-q")]
    [Description("Find query")]
    public string Query { get; set; } = "";

    [CommandOption("--exec")]
    [Description(
        "Run a program for each match. Pass its arguments after --; {} represents the file path."
    )]
    public string? Exec { get; set; }
}

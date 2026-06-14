using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Cli.Find;

public class FindSettings : TagDataActionSettings
{
    [CommandOption("--query|-q")]
    [Description("Find query")]
    public string Query { get; set; } = "";
}

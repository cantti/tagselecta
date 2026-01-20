using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.Commands.Cli.Find;

public class FindSettings : CliSettings
{
    [CommandOption("--query|-q")]
    [Description("Find query")]
    public string Query { get; set; } = "";
}

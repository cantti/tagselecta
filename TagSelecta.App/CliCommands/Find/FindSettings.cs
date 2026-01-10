using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.App.CliCommands.Find;

public class FindSettings : BaseSettings
{
    [CommandOption("--query|-q")]
    [Description("Find query")]
    public string Query { get; set; } = "";
}

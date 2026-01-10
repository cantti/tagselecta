using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.CliCommands.Split;

public class SplitSettings : BaseSettings
{
    [CommandOption("--separator|-s")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Separator { get; set; }
}

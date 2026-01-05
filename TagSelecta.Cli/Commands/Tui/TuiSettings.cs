using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.Tui;

public class TuiSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

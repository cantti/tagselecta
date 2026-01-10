using Spectre.Console.Cli;

namespace TagSelecta.App.Tui;

public class TuiSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

using Spectre.Console.Cli;

namespace TagSelecta.Cli;

public abstract class BaseSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];

    [CommandOption("--yes")]
    public bool Yes { get; set; }
}

using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands;

public abstract class BaseSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

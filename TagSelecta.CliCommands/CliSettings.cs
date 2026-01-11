using Spectre.Console.Cli;

namespace TagSelecta.CliCommands;

public abstract class CliSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

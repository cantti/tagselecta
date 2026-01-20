using Spectre.Console.Cli;

namespace TagSelecta.Commands.Cli;

public abstract class CliSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

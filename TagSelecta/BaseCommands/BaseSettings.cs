using Spectre.Console.Cli;

namespace TagSelecta.BaseCommands;

public abstract class BaseSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

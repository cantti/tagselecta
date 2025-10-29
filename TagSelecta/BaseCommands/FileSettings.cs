using Spectre.Console.Cli;

namespace TagSelecta.BaseCommands;

public abstract class FileSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

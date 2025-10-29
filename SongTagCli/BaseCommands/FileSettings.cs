using Spectre.Console.Cli;

namespace SongTagCli.BaseCommands;

public abstract class FileSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

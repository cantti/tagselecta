using Spectre.Console.Cli;

namespace TagSelecta.Commands;

public abstract class BaseSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

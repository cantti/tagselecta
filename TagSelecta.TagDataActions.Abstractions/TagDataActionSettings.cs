using Spectre.Console.Cli;

namespace TagSelecta.TagDataActions.Abstractions;

public abstract class TagDataActionSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

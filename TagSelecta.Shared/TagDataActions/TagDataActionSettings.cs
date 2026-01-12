using Spectre.Console.Cli;

namespace TagSelecta.Shared.TagDataActions;

public abstract class TagDataActionSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

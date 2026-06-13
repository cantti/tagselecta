using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.TagDataActions.Abstractions;

public abstract class TagDataActionSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];

    [CommandOption("--no-recursive")]
    [Description("Do not scan subdirectories.")]
    public bool NoRecursive { get; set; }

    [CommandOption("--yes")]
    [Description("Skip confirmation before writing changes to files.")]
    public bool Yes { get; set; }
}

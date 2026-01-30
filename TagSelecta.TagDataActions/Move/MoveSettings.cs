using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Move;

public class MoveSettings : TagDataActionSettings
{
    [CommandOption("--template|-t", isRequired: true)]
    [Description("Template. For example: {{ track }} - {{ title }}.{{ ext }}")]
    public required string Template { get; set; }

    [CommandOption("--keepemptydirs|-k")]
    [Description("Keep empty directories.")]
    public bool KeepEmptyDirectories { get; set; }

    [CommandOption("--donotmoveother|-d")]
    [Description("Do not move other files.")]
    public bool DoNotMoveOtherFiles { get; set; }
}

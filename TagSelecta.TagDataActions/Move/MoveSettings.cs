using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.Move;

public class MoveSettings : TagDataActionSettings
{
    [CommandOption("--template|-t", isRequired: true)]
    [Description("Template. For example: {{ date }} - {{ album }}")]
    public required string Template { get; set; }
}

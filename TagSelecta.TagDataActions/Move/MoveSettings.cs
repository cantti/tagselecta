using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.Move;

public class MoveSettings : TagDataActionSettings
{
    [CommandOption("--template|-t")]
    [Description("Template. For example: {{ date }} - {{ album }}")]
    public string Template { get; set; } = "";

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Template))
        {
            return ValidationResult.Error("Template is required");
        }
        return base.Validate();
    }
}

using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.RenameFile;

public class RenameFileSettings : TagDataActionSettings
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

    public override void ParseTuiArgs(IEnumerable<TagDataActionArg> args)
    {
        Template =
            args.FirstOrDefault(x => x.Key is "template" or "t" or "0")?.Value
            ?? throw new InvalidOperationException("Template is required.");
        if (string.IsNullOrWhiteSpace(Template))
        {
            throw new InvalidOperationException("Template is required.");
        }
    }
}

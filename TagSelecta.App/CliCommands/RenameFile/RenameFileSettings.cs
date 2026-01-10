using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.App.Tui.TuiCommands;

namespace TagSelecta.App.CliCommands.RenameFile;

public class RenameFileSettings : BaseSettings
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

    public override void ParseTuiArgs(IEnumerable<Arg> args)
    {
        Template =
            args.FirstOrDefault(x => x.Key == "template" || x.Key == "t" || x.Key == "arg0")?.Value
            ?? throw new InvalidOperationException("Template is required.");
    }
}

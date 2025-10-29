using System.ComponentModel;
using System.Text.RegularExpressions;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Misc;
using TagSelecta.Tagging;
using TagSelecta.TagTemplate;
using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.Actions;

public class RenameFileSettings : FileSettings
{
    [CommandOption("--template|-t")]
    [Description("Template. For example: {{ year }} - {{ album }}")]
    public string Template { get; set; } = "";

    [CommandOption("--dry-run")]
    public bool DryRun { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Template))
        {
            return ValidationResult.Error("Template is required");
        }
        return base.Validate();
    }
}

public class RenameFileAction : IAction<RenameFileSettings>
{
    public void Execute(ActionContext<RenameFileSettings> context)
    {
        var dir = Path.GetDirectoryName(context.File)!;

        var tagData = Tagger.ReadTags(context.File);

        var newName = TagTemplateFormatter
            .Format(context.Settings.Template, new TagTemplateContext(tagData, context.File))
            .CleanFileName();

        newName = Path.ChangeExtension(newName, Path.GetExtension(context.File));

        var newPath = Path.Combine(dir, newName);

        if (newPath == context.File)
        {
            context.Console.MarkupLine("File name already matches the desired format.");
            context.Skip();
            return;
        }

        context.Console.MarkupLine("File rename details:");
        context.Console.MarkupLine($"  Old: {context.File.EscapeMarkup()}");
        context.Console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.Settings.DryRun)
        {
            context.Console.MarkupLine("Dry run.");
            context.Skip();
            return;
        }

        if (context.ConfirmPrompt())
        {
            File.Move(context.File, newPath);
        }
    }
}

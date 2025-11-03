using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Misc;
using TagSelecta.Tagging;
using TagSelecta.TagTemplate;

namespace TagSelecta.Actions;

public class RenameFileSettings : FileSettings
{
    [CommandOption("--template|-t")]
    [Description("Template. For example: {year} - {album}")]
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

public class RenameFileAction(IAnsiConsole console) : FileAction<RenameFileSettings>
{
    public override Task Execute(ActionContext<RenameFileSettings> context)
    {
        var dir = Path.GetDirectoryName(context.File)!;

        var tagData = Tagger.ReadTags(context.File);

        var newName = TagTemplateFormatter
            .Format(context.Settings.Template, tagData)
            .CleanFileName();

        newName = $"{newName}{Path.GetExtension(context.File)}";

        var newPath = Path.Combine(dir, newName);

        if (newPath == context.File)
        {
            console.MarkupLine("File name already matches the desired format.");
            context.Skip();
            return Task.CompletedTask;
        }

        console.MarkupLine("File rename details:");
        console.MarkupLine($"  Old: {context.File.EscapeMarkup()}");
        console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.ConfirmPrompt())
        {
            File.Move(context.File, newPath);
        }
        return Task.CompletedTask;
    }
}

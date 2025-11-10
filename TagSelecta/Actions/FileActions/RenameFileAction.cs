using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Actions.FileActions;

public class RenameFileSettings : BaseSettings
{
    [CommandOption("--template|-t")]
    [Description("Template. For example: {{ year }} - {{ album }}")]
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

public class RenameFileAction(IAnsiConsole console) : IFileAction<RenameFileSettings>
{
    public Task ProcessFile(FileActionContext<RenameFileSettings> context)
    {
        var dir = Path.GetDirectoryName(context.CurrentFile)!;

        var tagData = Tagger.ReadTags(context.CurrentFile);

        var newName = TagDataFormatter.Format(context.Settings.Template, tagData);

        newName = FileHelper.CleanFileName(newName);

        newName = $"{newName}{Path.GetExtension(context.CurrentFile)}";

        var newPath = Path.Combine(dir, newName);

        if (newPath == context.CurrentFile)
        {
            console.MarkupLine("File name already matches the desired format.");
            return Task.CompletedTask;
        }

        console.MarkupLine("File rename details:");
        console.MarkupLine($"  Old: {context.CurrentFile.EscapeMarkup()}");
        console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.ConfirmPrompt())
        {
            File.Move(context.CurrentFile, newPath);
            return Task.CompletedTask;
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}

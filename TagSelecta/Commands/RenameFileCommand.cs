using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

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

public class RenameFileCommand(IAnsiConsole console) : FileProcessingCommand<RenameFileSettings>(console)
{
    protected override void ProcessFile()
    {
        var dir = Path.GetDirectoryName(CurrentFile)!;

        var tagData = Tagger.ReadTags(CurrentFile);

        var newName = Formatter.Format(Settings.Template, tagData);

        newName = FileHelper.CleanFileName(newName);

        newName = $"{newName}{Path.GetExtension(CurrentFile)}";

        var newPath = Path.Combine(dir, newName);

        if (newPath == CurrentFile)
        {
            Console.MarkupLine("File name already matches the desired format.");
            Skip();
            return;
        }

        Console.MarkupLine("File rename details:");
        Console.MarkupLine($"  Old: {CurrentFile.EscapeMarkup()}");
        Console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (ConfirmPrompt())
        {
            File.Move(CurrentFile, newPath);
        }
    }
}

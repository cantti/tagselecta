using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Formatting;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class RenameFileSettings : FileSettings
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

public class RenameFileCommand(IAnsiConsole console) : FileCommand<RenameFileSettings>(console)
{
    protected override void Execute(string file, int index)
    {
        var dir = Path.GetDirectoryName(file)!;

        var tagData = Tagger.ReadTags(file);

        var newName = Formatter.Format(Settings.Template, tagData);

        newName = FileHelper.CleanFileName(newName);

        newName = $"{newName}{Path.GetExtension(file)}";

        var newPath = Path.Combine(dir, newName);

        if (newPath == file)
        {
            Console.MarkupLine("File name already matches the desired format.");
            Skip();
            return;
        }

        Console.MarkupLine("File rename details:");
        Console.MarkupLine($"  Old: {file.EscapeMarkup()}");
        Console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (ConfirmPrompt())
        {
            File.Move(file, newPath);
        }
    }
}

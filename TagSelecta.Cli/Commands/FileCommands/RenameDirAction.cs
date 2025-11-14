using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands;
using TagSelecta.Cli.IO;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.FileCommands;

public class RenameDirSettings : BaseSettings
{
    [CommandOption("--template|-t")]
    [Description("Template. For example: {{ year } - {{ album }}")]
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

public class RenameDirAction(IAnsiConsole console) : FileAction<RenameDirSettings>
{
    private readonly List<string> _renamed = [];

    protected override void ProcessFile(FileActionContext<RenameDirSettings> context)
    {
        var dir = Path.GetDirectoryName(context.CurrentFile)!;
        if (_renamed.Contains(dir))
        {
            return;
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(context.CurrentFile);

        var formatter = new TagDataFormatter(tagData, context.CurrentFile);

        var newName = formatter.Format(context.Settings.Template);

        newName = FileHelper.CleanFileName(newName);

        var newPath = GetNewPath(dir, newName);

        if (newPath == dir)
        {
            console.MarkupLine("Directory name already matches the desired format.");
            return;
        }

        if (Directory.Exists(newPath))
        {
            throw new TagSelectaException($"Target directory already exists: {newPath}.");
        }

        console.MarkupLine("Directory rename details:");
        console.MarkupLine($"  Old: {dir.EscapeMarkup()}");
        console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.ConfirmPrompt())
        {
            Directory.Move(dir, newPath);
            CommandHelper.PrintStatusSuccess(console);
        }
    }

    private static string GetNewPath(string dirPath, string newName)
    {
        string parentDir = Path.GetDirectoryName(dirPath)!;
        return Path.Combine(parentDir, newName);
    }
}

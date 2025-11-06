using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Formatting;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class RenameDirSettings : FileSettings
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

public class RenameDirCommand(IAnsiConsole console) : FileCommand<RenameDirSettings>(console)
{
    private readonly List<string> _renamed = [];

    protected override Task Execute(string file, int index)
    {
        var dir = Path.GetDirectoryName(file)!;
        if (_renamed.Contains(dir))
        {
            Skip();
            return Task.CompletedTask;
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(file);

        var newName = Formatter.Format(Settings.Template, tagData);

        newName = FileHelper.CleanFileName(newName);

        var newPath = GetNewPath(dir, newName);

        if (newPath == dir)
        {
            Console.MarkupLine("Directory name already matches the desired format.");
            Skip();
            return Task.CompletedTask;
        }

        if (Directory.Exists(newPath))
        {
            throw new ActionException($"Target directory already exists: {newPath}.");
        }

        Console.MarkupLine("Directory rename details:");
        Console.MarkupLine($"  Old: {dir.EscapeMarkup()}");
        Console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (ConfirmPrompt())
        {
            Directory.Move(dir, newPath);
        }
        return Task.CompletedTask;
    }

    private static string GetNewPath(string dirPath, string newName)
    {
        string parentDir = Path.GetDirectoryName(dirPath)!;
        return Path.Combine(parentDir, newName);
    }
}

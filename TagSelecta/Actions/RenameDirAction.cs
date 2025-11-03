using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Misc;
using TagSelecta.Tagging;
using TagSelecta.TagTemplate;

namespace TagSelecta.Actions;

public class RenameDirSettings : FileSettings
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

public class RenameDirAction(IAnsiConsole console, ActionContext<RenameDirSettings> context)
    : IFileAction<RenameDirSettings>
{
    private readonly List<string> _renamed = [];

    public Task Execute(string file, int index)
    {
        var dir = Path.GetDirectoryName(file)!;
        if (_renamed.Contains(dir))
        {
            context.Skip();
            return Task.CompletedTask;
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(file);

        var newName = TagTemplateFormatter
            .Format(context.Settings.Template, tagData)
            .CleanFileName();

        var newPath = GetNewPath(dir, newName);

        if (newPath == dir)
        {
            console.MarkupLine("Directory name already matches the desired format.");
            context.Skip();
            return Task.CompletedTask;
        }

        if (Directory.Exists(newPath))
        {
            throw new ActionException($"Target directory already exists: {newPath}.");
        }

        console.MarkupLine("Directory rename details:");
        console.MarkupLine($"  Old: {dir.EscapeMarkup()}");
        console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.ConfirmPrompt())
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

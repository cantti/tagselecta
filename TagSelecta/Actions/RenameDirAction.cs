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

public class RenameDirAction(IAnsiConsole console) : FileAction<RenameDirSettings>
{
    private readonly List<string> _renamed = [];

    public override void Execute(ActionContext<RenameDirSettings> context)
    {
        var dir = Path.GetDirectoryName(context.File)!;
        if (_renamed.Contains(dir))
        {
            context.Skip();
            return;
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(context.File);

        var newName = TagTemplateFormatter
            .Format(context.Settings.Template, new TagTemplateContext(tagData, context.File))
            .CleanFileName();

        var newPath = GetNewPath(dir, newName);

        if (newPath == dir)
        {
            console.MarkupLine("Directory name already matches the desired format.");
            context.Skip();
            return;
        }

        if (Directory.Exists(newPath))
        {
            throw new ActionException($"Target directory already exists: {newPath}.");
        }

        console.MarkupLine("Directory rename details:");
        console.MarkupLine($"  Old: {dir.EscapeMarkup()}");
        console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.Settings.DryRun)
        {
            console.MarkupLine("Dry run.");
            context.Skip();
        }
        else if (context.ConfirmPrompt())
        {
            Directory.Move(dir, newPath);
        }
    }

    private static string GetNewPath(string dirPath, string newName)
    {
        string parentDir = Path.GetDirectoryName(dirPath)!;
        return Path.Combine(parentDir, newName);
    }
}

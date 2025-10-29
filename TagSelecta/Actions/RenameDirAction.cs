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

public class RenameDirAction : IAction<RenameDirSettings>
{
    private readonly List<string> _renamed = [];

    public void Execute(ActionContext<RenameDirSettings> context)
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
            context.Console.MarkupLine("Directory name already matches the desired format.");
            context.Skip();
            return;
        }

        if (Directory.Exists(newPath))
        {
            throw new ActionException($"Target directory already exists: {newPath}.");
        }

        context.Console.MarkupLine("Directory rename details:");
        context.Console.MarkupLine($"  Old: {dir.EscapeMarkup()}");
        context.Console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (context.Settings.DryRun)
        {
            context.Console.MarkupLine("Dry run.");
            context.Skip();
            return;
        }

        if (context.ConfirmPrompt())
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

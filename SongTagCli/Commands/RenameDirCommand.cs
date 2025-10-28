using System.ComponentModel;
using System.Text;
using SmartFormat.Core.Settings;
using SongTagCli.BaseCommands;
using SongTagCli.Misc;
using SongTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SongTagCli.Commands;

public class RenameDirSettings : FileProcessingSettings
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

public class RenameDirCommand(IAnsiConsole console)
    : FileProcessingCommandBase<RenameDirSettings>(console)
{
    private readonly List<string> _renamed = [];

    protected override Task<ResultStatus> ProcessFileAsync(
        RenameDirSettings settings,
        List<string> files,
        string file
    )
    {
        var dir = Path.GetDirectoryName(file)!;
        if (_renamed.Contains(dir))
        {
            return Task.FromResult(ResultStatus.Skipped);
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(file);

        var newName = TagTemplateFormatter.Format(
            settings.Template,
            new TagTemplateContext(tagData, file)
        );

        var newPath = GetNewPath(dir, newName);

        if (newPath == dir)
        {
            Console.MarkupLine("Directory name already matches the desired format.");
            return Task.FromResult(ResultStatus.Skipped);
        }

        if (Directory.Exists(newPath))
        {
            Console.MarkupLine($"Target directory already exists: {newPath.EscapeMarkup()}.");
            return Task.FromResult(ResultStatus.Error);
        }

        Console.MarkupLine("Directory rename details:");
        Console.MarkupLine($"  Old: {dir.EscapeMarkup()}");
        Console.MarkupLine($"  New: {newPath.EscapeMarkup()}");

        if (settings.DryRun)
        {
            Console.MarkupLine("Dry run.");
            return Task.FromResult(ResultStatus.Success);
        }

        if (Confirm())
        {
            Directory.Move(dir, newPath);
            return Task.FromResult(ResultStatus.Success);
        }
        return Task.FromResult(ResultStatus.Skipped);
    }

    private static string GetNewPath(string dirPath, string newName)
    {
        string parentDir = Path.GetDirectoryName(dirPath)!;
        return Path.Combine(parentDir, newName);
    }
}

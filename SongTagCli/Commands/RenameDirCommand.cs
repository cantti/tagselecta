using System.ComponentModel;
using System.Text;
using Scriban;
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
    public string? Template { get; set; }

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
    protected override bool PrintFileAfterProcessing => false;

    private readonly List<string> _renamed = [];

    protected override async Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        RenameDirSettings settings,
        List<string> files,
        string file
    )
    {
        var dir = Path.GetDirectoryName(file)!;
        if (_renamed.Contains(dir))
        {
            return new ProcessFileResult(ProcessFileResultStatus.Skipped);
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(file);

        var template = Template.Parse(settings.Template);
        var newName = template.Render(new TagTemplateContext(tagData, file));

        var newPath = GetNewPath(dir, newName);

        if (newPath == dir)
        {
            return await Task.FromResult(
                ProcessFileResult.Skipped("Directory name already matches the desired format.")
            );
        }

        if (Directory.Exists(newPath))
        {
            return await Task.FromResult(
                ProcessFileResult.Error(
                    $"Target directory already exists: {newPath.EscapeMarkup()}."
                )
            );
        }

        var sb = new StringBuilder();
        sb.AppendLine("Directory rename details:");
        sb.AppendLine($"  Old: {dir.EscapeMarkup()}");
        sb.AppendLine($"  New: {newPath.EscapeMarkup()}");

        if (settings.DryRun)
        {
            sb.AppendLine("Dry run.");
            return await Task.FromResult(ProcessFileResult.Success(sb.ToString()));
        }

        Directory.Move(dir, newPath);

        return await Task.FromResult(ProcessFileResult.Success(sb.ToString()));
    }

    private static string GetNewPath(string dirPath, string newName)
    {
        string parentDir = Path.GetDirectoryName(dirPath)!;
        return Path.Combine(parentDir, newName);
    }
}

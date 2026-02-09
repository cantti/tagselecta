using Spectre.Console;
using Spectre.Console.Cli;
using TagLib.Ape;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Cli.ExecuteTagDataAction;

public class ExecuteTagDataActionCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    IFileSystem fs,
    ITagger tagger
) : AsyncCommand<TSettings>
    where TSettings : TagDataActionSettings
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        try
        {
            if (!ValidateOptions(context))
            {
                return 1;
            }

            if (!await action.BeforeExecuteAsync(settings, ct))
            {
                return 0;
            }

            var files = audioFileScanner
                .SearchAndRead(settings.Path, ct)
                .Select(x => new TagDataActionTarget(x.Path, x.TagData))
                .ToList();

            console.WriteLine($"Total {files.Count} files found");

            await ExecuteAction(settings, files, ct);

            if (
                !await console.ConfirmAsync(
                    $"{files.Count(x => x.HasChanges)} pending files. {files.Count(x => x.Exception is not null)} errors. Continue?",
                    cancellationToken: ct
                )
            )
            {
                return 0;
            }

            Write(files, ct);

            console.MarkupLineInterpolated(
                $"Completed. {files.Count(x => x.Exception is not null)} errors."
            );
        }
        catch (OperationCanceledException) { }

        return 0;
    }

    private async Task ExecuteAction(
        TSettings settings,
        List<TagDataActionTarget> files,
        CancellationToken ct
    )
    {
        var executor = new TagDataActionTargetExecutor();
        await console
            .Progress()
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Processing metadata...", maxValue: files.Count);
                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    await executor.ExecuteTagDataAction(file, action, settings, ct);
                    task.Increment(1);
                }
            });
    }

    private void Write(List<TagDataActionTarget> files, CancellationToken ct)
    {
        var writer = new TagDataActionTargetWriter(fs, tagger);
        console
            .Progress()
            .AutoClear(true)
            .Start(ctx =>
            {
                var filesToWrite = files.Where(x => x.HasChanges).ToList();
                var task = ctx.AddTask("Writing files...", maxValue: filesToWrite.Count);
                foreach (var file in filesToWrite)
                {
                    ct.ThrowIfCancellationRequested();
                    writer.Write(file);
                    task.Increment(1);
                }
            });
    }

    private bool ValidateOptions(CommandContext context)
    {
        var unknownOptions = context
            .Remaining.Parsed.Select(kvp => kvp.Key)
            .Concat(context.Remaining.Raw)
            .ToList();
        if (unknownOptions.Count != 0)
        {
            console.MarkupLine("[red]Unknown option(s) provided:[/]");
            foreach (var option in unknownOptions)
            {
                console.WriteLine($"  {option}", new Style(Color.Yellow));
            }

            return false;
        }

        return true;
    }
}

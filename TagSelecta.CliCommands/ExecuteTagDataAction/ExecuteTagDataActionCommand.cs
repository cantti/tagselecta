using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.CliCommands.ExecuteTagDataAction;

public class ExecuteTagDataActionCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    IFileSystem fs
) : AsyncCommand<TSettings>
    where TSettings : TagDataActionSettings
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        if (!ValidateOptions(context))
        {
            return 1;
        }

        if (!await action.BeforeExecuteAsync(settings, ct))
        {
            return 0;
        }

        var operations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        console.WriteLine($"Total {operations.Count} files found");

        await ExecuteAction(settings, operations, ct);

        if (
            !await console.ConfirmAsync(
                $"Continue with writing {operations.Count(x => x.HasChanges)}?",
                cancellationToken: ct
            )
        )
        {
            return 0;
        }

        Write(operations, ct);

        console.MarkupLineInterpolated(
            $"{operations.Count(x => x.Exception is null)}/{operations.Count} files written"
        );

        return 0;
    }

    private async Task ExecuteAction(
        TSettings settings,
        List<TagDataOperation> operations,
        CancellationToken ct
    )
    {
        await console
            .Progress()
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Processing metadata...", maxValue: operations.Count);
                foreach (var operation in operations)
                {
                    try
                    {
                        await action.ExecuteAsync(operation, operations, settings, ct);
                        operation.CheckForChanges();
                    }
                    catch (Exception ex)
                    {
                        operation.MarkError(ex);
                    }
                    task.Increment(1);
                }
            });
    }

    private void Write(List<TagDataOperation> operations, CancellationToken ct)
    {
        console
            .Progress()
            .AutoClear(true)
            .Start(ctx =>
            {
                var operationsToWrite = operations.Where(x => x.HasChanges).ToList();
                var task = ctx.AddTask("Writing files...", maxValue: operationsToWrite.Count);
                foreach (var operation in operationsToWrite)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.3));
                    ct.ThrowIfCancellationRequested();
                    operation.Write(tagger, fs);
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
            console.MarkupLine($"[red]Unknown option(s) provided:[/]");
            foreach (var option in unknownOptions)
            {
                console.WriteLine($"  {option}", new Style(Color.Yellow));
            }
            return false;
        }
        return true;
    }
}

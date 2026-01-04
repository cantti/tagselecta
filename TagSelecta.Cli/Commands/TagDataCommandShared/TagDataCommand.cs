using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.TagDataCommandShared.BulkWrite;
using TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public class TagDataCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    IInteractiveWriter interactiveWriter,
    IBulkWriter bulkWriter
) : AsyncCommand<TSettings>
    where TSettings : BaseSettings
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

        AltScreen.Enter();

        // todo add progress
        if (!await action.BeforeProcessTagDataAsync(settings))
        {
            return 0;
        }

        var operations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        console.WriteLine(operations.Count + " files found");

        await ProcessTagData(settings, operations);

        if (settings.Yes)
        {
            bulkWriter.WriteAll(operations);
        }
        else
        {
            interactiveWriter.Start(operations);
        }

        AltScreen.Exit();

        console.MarkupLineInterpolated(
            $"{operations.Count(x => x.Status == TagDataOperationStatus.Written)}/{operations.Count} files written"
        );

        var errorsCount = operations.Count(x => x.Status == TagDataOperationStatus.Failed);

        if (errorsCount > 0)
        {
            console.MarkupLineInterpolated($"{errorsCount} errors");
        }

        return 0;
    }

    private async Task ProcessTagData(TSettings settings, List<TagDataOperation> operations)
    {
        await console
            .Progress()
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Processing metadata...", maxValue: operations.Count);
                var progressLock = new object();
                var allFiles = operations
                    .Select(x => new FileWithTagData(x.Path, x.TagData))
                    .ToList();
                await Parallel.ForEachAsync(
                    operations,
                    async (operation, _) =>
                    {
                        try
                        {
                            await action.ProcessTagDataAsync(
                                new FileWithTagData(operation.Path, operation.TagData),
                                allFiles,
                                settings
                            );
                            operation.CheckForChanges();
                        }
                        catch (Exception ex)
                        {
                            operation.MarkError(ex);
                        }
                        lock (progressLock)
                        {
                            task.Increment(1);
                        }
                    }
                );
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

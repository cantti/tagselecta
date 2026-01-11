using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.CliCommands.ExecuteTagDataAction;

public class ExecuteTagDataActionCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner
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

        if (!await action.BeforeProcessTagDataAsync(settings, ct))
        {
            return 0;
        }

        var operations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        console.WriteLine(operations.Count + " files found");

        await ProcessTagData(settings, operations, ct);

        // write here

        console.MarkupLineInterpolated(
            $"{operations.Count(x => x.Exception is null)}/{operations.Count} files written"
        );

        return 0;
    }

    private async Task ProcessTagData(
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
                var progressLock = new object();
                await Parallel.ForEachAsync(
                    operations,
                    ct,
                    async (operation, _) =>
                    {
                        try
                        {
                            await action.ProcessTagDataAsync(operation, operations, settings, ct);
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

using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Common;

public class TagDataCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger
) : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    public override async Task<int> ExecuteAsync(
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
            WriteAll(operations);
        }
        else
        {
            InteractiveWrite(operations);
        }

        AltScreen.Exit();

        console.MarkupLineInterpolated(
            $"{operations.Count(x => x.IsSaved && x.Exception is null)}/{operations.Count} files written"
        );

        var errorsCount = operations.Count(x => x.IsSaved && x.Exception is not null);

        if (errorsCount > 0)
        {
            console.MarkupLineInterpolated($"{errorsCount} errors");
        }

        return 0;
    }

    private async Task ProcessTagData(TSettings settings, List<TagDataOperation> operations)
    {
        console.Clear();
        await console
            .Progress()
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var progressLock = new object();
                var allFiles = operations
                    .Select(x => new FileWithTagData(x.Path, x.TagData))
                    .ToList();
                var task = ctx.AddTask("Processing metadata...", maxValue: operations.Count);
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
                            operation.HasChanges = !TagDataComparer.AreEqual(
                                operation.TagData,
                                operation.OriginalTagData
                            );
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

    private void InteractiveWrite(List<TagDataOperation> operations)
    {
        var index = 0;
        var filter = false;

        var filtered = operations;

        while (true)
        {
            console.Clear();

            index = CommandHelper.ClampIndex(index, filtered.Count);

            TagDataOperation? current = null;

            if (filtered.Count > 0)
            {
                current = filtered[index];

                CommandHelper.PrintCurrentFile(console, current.Path, index, filtered.Count);

                var areEqual = TagDataComparer.AreEqual(current.OriginalTagData, current.TagData);
                if (areEqual)
                {
                    TagDataPrinter.PrintTagData(console, current.TagData);
                }
                else
                {
                    TagDataPrinter.PrintComparison(
                        console,
                        current.OriginalTagData,
                        current.TagData
                    );
                }
                if (current.Exception is not null)
                {
                    console.MarkupLine("[red]Error processing file:[/]");
                    console.WriteException(current.Exception, ExceptionFormats.ShortenEverything);
                }
            }
            else
            {
                console.WriteLine("No files with changes");
            }
            var cmd = CommandHelper.ReadNavigationCommand(console);
            if (cmd == UserInput.Next)
            {
                index++;
            }
            else if (cmd == UserInput.Previous)
            {
                index--;
            }
            else if (cmd == UserInput.WriteAll)
            {
                WriteAll(operations);
            }
            else if (cmd == UserInput.Write)
            {
                if (current is not null && !current.IsSaved && current.HasChanges)
                {
                    WriteTags(current);
                }
            }
            else if (cmd == UserInput.ToggleFilter)
            {
                filter = !filter;
                filtered = filter ? operations.Where(x => x.HasChanges).ToList() : operations;
                index = 0;
            }
            else
            {
                break;
            }
        }
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
                console.MarkupLine($"  [yellow]{option.EscapeMarkup()}[/]");
            }
            return false;
        }
        return true;
    }

    private void WriteAll(List<TagDataOperation> operations)
    {
        console.Clear();
        var operationsToWrite = operations.Where(x => !x.IsSaved && x.HasChanges).ToList();
        console
            .Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask("Writing metadata...", maxValue: operationsToWrite.Count);
                for (var i = 0; i < operationsToWrite.Count; i++)
                {
                    var operation = operationsToWrite[i];
                    WriteTags(operation);
                    task.Description =
                        $"Writing metadata {i + 1} of {operationsToWrite.Count}({operation.Path.EscapeMarkup()})";
                    task.Increment(1);
                }
            });
    }

    private void WriteTags(TagDataOperation operation)
    {
        try
        {
            tagger.WriteTags(operation.Path, operation.TagData);
            operation.MarkSaved();
        }
        catch (Exception ex)
        {
            operation.MarkError(ex);
        }
    }
}

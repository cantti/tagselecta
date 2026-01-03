using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public class TagDataCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    IUserActionReader userActionReader
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
            var navigation = userActionReader.RenderNavigation(filter);

            var bodySize = (Console.WindowHeight - navigation.Size) / 2;
            var layout = new Layout("root").SplitRows(
                new Layout("header").Size(navigation.Size),
                new Layout("top").Size(bodySize),
                new Layout("body").Size(bodySize)
            );

            console.Clear();

            index = CommandHelper.ClampIndex(index, filtered.Count);

            TagDataOperation? current = null;

            if (filtered.Count > 0)
            {
                // -2 to compensate header and add empty row
                layout["top"].Update(RenderFilePathList(filtered, index, bodySize - 2));

                current = filtered[index];

                var tagDataRenderable = current.HasChanges
                    ? TagDataPrinter.PrintComparison(
                        console,
                        current.OriginalTagData,
                        current.TagData
                    )
                    : TagDataPrinter.PrintTagData(console, current.TagData);

                layout["body"].Update(tagDataRenderable);

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

            layout["header"].Update(navigation.Content);

            console.Write(layout);

            var cmd = userActionReader.Read();
            if (cmd == UserAction.Next)
            {
                index++;
            }
            else if (cmd == UserAction.Previous)
            {
                index--;
            }
            else if (cmd == UserAction.WriteAll)
            {
                WriteAll(operations);
            }
            else if (cmd == UserAction.Write)
            {
                if (current is not null && !current.IsSaved && current.HasChanges)
                {
                    WriteTags(current);
                }
            }
            else if (cmd == UserAction.ToggleFilter)
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

    private IRenderable RenderFilePathList(
        List<TagDataOperation> operations,
        int index,
        int windowSize
    )
    {
        if (operations.Count <= 0)
        {
            return new Text("");
        }

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var windowStart = index - (windowSize / 2);

        // clamp so we dont go before 0 or past the last possible full window start
        var maxStart = Math.Max(0, operations.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, operations.Count - windowStart);

        var items = new List<IRenderable>();

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var path = Path.GetRelativePath(
                Environment.CurrentDirectory,
                operations[itemIndex].Path
            );
            var lineNumber = (itemIndex + 1).ToString().PadLeft(4);
            var modifiedMarker = operations[itemIndex].HasChanges ? "*" : " ";
            var text = $"{lineNumber} {modifiedMarker} {path}";
            text = text.Substring(0, Math.Min(text.Length, Console.WindowWidth))
                .PadRight(Console.WindowWidth);
            var style = new Style(background: index == itemIndex ? Color.Grey : null);
            items.Add(new Text(text, style));
        }

        return new Rows(new Text("Files:", new Style(Color.Yellow)), new Rows(items));
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
            operation.HasChanges = false;
            operation.MarkSaved();
        }
        catch (Exception ex)
        {
            operation.MarkError(ex);
        }
    }
}

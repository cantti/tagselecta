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

    private void InteractiveWrite(List<TagDataOperation> operations)
    {
        console.Cursor.Hide();

        var selectedIndex = 0;
        var filterEnabled = false;

        var filtered = operations;

        while (true)
        {
            console.Clear();

            var navigation = userActionReader.RenderNavigation();

            var filesContentSize = Math.Min(
                (Console.WindowHeight - navigation.Size) / 2,
                // +2 for navigation and add empty row
                filtered.Count + 2
            );
            const string headerLayoutKey = "navigation";
            const string filesLayoutKey = "files";
            const string tagDataLayoutKey = "body";

            var layout = new Layout("root").SplitRows(
                new Layout(headerLayoutKey).Size(navigation.Size),
                new Layout(filesLayoutKey).Size(filesContentSize),
                new Layout(tagDataLayoutKey)
            );

            selectedIndex = Math.Clamp(selectedIndex, 0, filtered.Count - 1);

            TagDataOperation? operation = null;

            if (filtered.Count > 0)
            {
                // -2 to compensate navigation and add empty row
                layout[filesLayoutKey]
                    .Update(
                        RenderFileList(filtered, selectedIndex, filesContentSize - 2, filterEnabled)
                    );

                operation = filtered[selectedIndex];

                var tagDataRenderable = operation.HasChanges
                    ? TagDataPrinter.PrintComparison(operation.OriginalTagData, operation.TagData)
                    : TagDataPrinter.PrintTagData(console, operation.TagData);

                if (operation.Exception is null)
                {
                    layout[tagDataLayoutKey].Update(tagDataRenderable);
                }
                else
                {
                    layout[tagDataLayoutKey]
                        .Update(
                            new Rows(
                                tagDataRenderable,
                                Text.NewLine,
                                new Text(
                                    $"Error: {operation.Exception.Message}",
                                    new Style(Color.Red)
                                )
                            )
                        );
                }
            }
            else
            {
                console.WriteLine("No files with changes");
            }

            layout[headerLayoutKey].Update(navigation.Content);

            console.Write(layout);

            var cmd = userActionReader.Read();
            if (cmd == UserAction.Next)
            {
                selectedIndex++;
            }
            else if (cmd == UserAction.Previous)
            {
                selectedIndex--;
            }
            else if (cmd == UserAction.WriteAll)
            {
                WriteAll(operations);
            }
            else if (cmd == UserAction.Write)
            {
                if (operation is { Status: TagDataOperationStatus.Pending, HasChanges: true })
                {
                    operation.Write(tagger);
                }
            }
            else if (cmd == UserAction.ToggleFilter)
            {
                filterEnabled = !filterEnabled;
                filtered = filterEnabled
                    ? operations.Where(x => x.HasChanges).ToList()
                    : operations;
                selectedIndex = 0;
            }
            else
            {
                break;
            }
        }
    }

    private IRenderable RenderFileList(
        List<TagDataOperation> operations,
        int selectedIndex,
        int windowSize,
        bool filter
    )
    {
        if (operations.Count == 0)
        {
            return Text.Empty;
        }

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var windowStart = selectedIndex - (windowSize / 2);

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
            var style = new Style(background: selectedIndex == itemIndex ? Color.Grey : null);
            items.Add(new Text(text, style));
        }

        return new Rows(
            new Text(
                $"Files ({operations.Count}{(filter ? ", filtered" : "")}):",
                new Style(Color.Yellow)
            ),
            new Rows(items)
        );
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
        var operationsToWrite = operations
            .Where(x => x is { Status: TagDataOperationStatus.Pending, HasChanges: true })
            .ToList();
        console
            .Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask("Writing metadata...", maxValue: operationsToWrite.Count);
                for (var i = 0; i < operationsToWrite.Count; i++)
                {
                    var operation = operationsToWrite[i];
                    operation.Write(tagger);
                    task.Description =
                        $"Writing metadata {i + 1} of {operationsToWrite.Count}({operation.Path.EscapeMarkup()})";
                    task.Increment(1);
                }
            });
    }
}

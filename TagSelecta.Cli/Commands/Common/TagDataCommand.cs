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
        AltScreen.Enter();

        if (!await action.BeforeProcessTagDataAsync(settings))
        {
            return 0;
        }

        var operations = CommandHelper
            .ScanAndRead(console, audioFileScanner, tagger, settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        foreach (var operation in operations)
        {
            try
            {
                await action.ProcessTagDataAsync(
                    new FileWithTagData { Path = operation.Path, TagData = operation.TagData },
                    operations
                        .Select(x => new FileWithTagData { Path = x.Path, TagData = x.TagData })
                        .ToList(),
                    settings
                );
            }
            catch (Exception ex)
            {
                operation.MarkError(ex);
            }
        }

        var index = 0;

        while (true)
        {
            console.Clear();

            index = CommandHelper.ClampIndex(index, operations.Count);

            var item = operations[index];

            CommandHelper.PrintCurrentFile(console, item.Path, index, operations.Count);

            var areEqual = TagDataComparer.AreEqual(item.OriginalTagData, item.TagData);
            if (areEqual)
            {
                TagDataPrinter.PrintTagData(console, item.TagData);
            }
            else
            {
                TagDataPrinter.PrintComparison(console, item.OriginalTagData, item.TagData);
            }
            if (item.Exception is not null)
            {
                console.MarkupLine("[red]Error processing file:[/]");
                console.WriteException(item.Exception, ExceptionFormats.ShortenEverything);
            }
            var cmd = CommandHelper.ReadNavigationCommand(
                console,
                item.Exception is null && !areEqual
            );
            if (item.Exception is null)
            {
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
                    WriteTags(item);
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (cmd == UserInput.Next)
                {
                    index++;
                }
                else if (cmd == UserInput.Previous)
                {
                    index--;
                }
                else
                {
                    break;
                }
            }
        }

        AltScreen.Exit();

        console.MarkupLineInterpolated(
            $"{operations.Count(x => x.IsSaved && x.Exception is null)}/{operations.Count} files written"
        );

        if (operations.Count > 0)
        {
            console.MarkupLineInterpolated(
                $"{operations.Count(x => x.IsSaved && x.Exception is not null)} errors"
            );
        }

        return 0;
    }

    private void WriteAll(List<TagDataOperation> items)
    {
        foreach (var item in items.Where(x => !x.IsSaved).ToList())
        {
            tagger.WriteTags(item.Path, item.TagData);
            item.MarkSaved();
        }
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

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

        console.Cursor.Hide();

        if (!await action.BeforeProcessTagDataAsync(settings))
        {
            return 0;
        }

        var items = CommandHelper
            .ScanAndRead(console, audioFileScanner, tagger, settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        foreach (var item in items)
        {
            try
            {
                await action.ProcessTagDataAsync(item, items, settings);
            }
            catch (Exception ex)
            {
                item.MarkError(ex);
            }
        }

        var index = 0;

        while (true)
        {
            console.Clear();

            index = CommandHelper.ClampIndex(index, items.Count);

            var item = items[index];

            CommandHelper.PrintCurrentFile(
                console,
                action.GetType().Name,
                item.Path,
                index,
                items.Count
            );

            if (item.Exception is null)
            {
                TagDataPrinter.PrintComparison(console, item.OriginalTagData, item.TagData);
                var cmd = CommandHelper.ReadNavigationCommand(console, true);
                if (cmd == NavCommand.Next)
                {
                    index++;
                }
                else if (cmd == NavCommand.Previous)
                {
                    index--;
                }
                else if (cmd == NavCommand.WriteAll)
                {
                    WriteAll(items);
                }
                else if (cmd == NavCommand.Write && item.HasChanges)
                {
                    tagger.WriteTags(item.Path, item.TagData);
                    item.MarkSaved();
                }
                else
                {
                    break;
                }
            }
            else
            {
                console.WriteException(item.Exception);
                var cmd = CommandHelper.ReadNavigationCommand(console, false);
                if (cmd == NavCommand.Next)
                {
                    index++;
                }
                else if (cmd == NavCommand.Previous)
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

        foreach (var item in items.Where(x => x.Exception is not null).ToList())
        {
            console.WriteException(item.Exception);
        }

        console.MarkupLineInterpolated($"{items.Count(x => x.IsSaved)} files written");

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
}

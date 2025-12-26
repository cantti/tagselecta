using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands.Common;

public class TagDataCommand<TSettings>(
    TagDataAction<TSettings> action,
    IAnsiConsole console,
    ITagger tagger
) : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    private List<Item> items = [];

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        AltScreen.Enter();

        console.Cursor.Hide();

        var files = CommandHelper.GetFiles(console, settings.Path);

        foreach (var file in files)
        {
            var tagData = tagger.ReadTags(file);
            var item = new Item(file, tagData);
            items.Add(item);
        }

        if (!await action.BeforeProcessTagDataAsync(settings))
        {
            return 0;
        }

        var index = 0;

        while (true)
        {
            console.Clear();

            index = ClampIndex(index, files.Count);

            var item = items[index];

            CommandHelper.PrintCurrentFile(
                console,
                action.GetType().Name,
                item.Path,
                index,
                files.Count
            );

            try
            {
                await action.ProcessTagDataAsync(item, items, settings);
                TagDataPrinter.PrintComparison(console, item.OriginalTagData, item.TagData);
                var cmd = ReadNavigationCommand(true);
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
                    WriteAll(settings);
                }
                else if (cmd == NavCommand.Write && item.HasChanges)
                {
                    tagger.WriteTags(item.Path, item.TagData);
                    item.Process();
                }
                else
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                item.Error(ex);
                console.Clear();
                console.WriteException(item.Exception);

                var cmd = ReadNavigationCommand(false);
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

        console.MarkupLineInterpolated($"{items.Count(x => x.IsProcessed)} files written");

        return 0;
    }

    private void WriteAll(TSettings settings)
    {
        foreach (var item in items.Where(x => !x.IsProcessed).ToList())
        {
            try
            {
                action.ProcessTagDataAsync(item, items, settings);
                tagger.WriteTags(item.Path, item.TagData);
                item.Process();
            }
            catch (Exception ex)
            {
                console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
            }
        }
    }

    private static int ClampIndex(int index, int count)
    {
        if (index < 0)
            return 0;

        if (index >= count)
            return count - 1;

        return index;
    }

    private NavCommand ReadNavigationCommand(bool showWrite)
    {
        Console.WriteLine(
            $"j = next, k = previous{(showWrite ? ", w = write, a = write all" : "")}, q = quit"
        );

        while (true)
        {
            var key = console.Input.ReadKey(true)?.KeyChar;

            switch (key)
            {
                case 'j':
                    return NavCommand.Next;
                case 'k':
                    return NavCommand.Previous;
                case 'w':
                    return NavCommand.Write;
                case 'a':
                    return NavCommand.WriteAll;
                case 'q':
                    return NavCommand.Quit;
            }
        }
    }
}

public enum NavCommand
{
    Next,
    Previous,
    Quit,
    Write,
    WriteAll,
}

public class Item
{
    public Item(string path, TagData tagData)
    {
        Path = path;
        TagData = tagData;
        OriginalTagData = tagData.Clone();
    }

    public string Path { get; private set; }
    public TagData TagData { get; private set; }
    public TagData OriginalTagData { get; private set; }
    public bool IsProcessed { get; private set; }
    public Exception Exception { get; private set; }

    public bool HasChanges => !TagDataComparer.AreEqual(TagData, OriginalTagData);

    public void Process()
    {
        IsProcessed = true;
        OriginalTagData = TagData.Clone();
    }

    public void Error(Exception ex)
    {
        IsProcessed = true;
        Exception = ex;
    }
}

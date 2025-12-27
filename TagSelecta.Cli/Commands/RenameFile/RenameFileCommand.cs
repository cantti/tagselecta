using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.RenameFile;

public class RenameFileCommand(
    IAnsiConsole console,
    ITagger tagger,
    IFileSystem fs,
    IAudioFileScanner audioFileScanner
) : Command<RenameFileSettings>
{
    public override int Execute(
        CommandContext context,
        RenameFileSettings settings,
        CancellationToken cancellationToken
    )
    {
        AltScreen.Enter();

        console.Cursor.Hide();

        var files = CommandHelper.ScanAndRead(console, audioFileScanner, tagger, settings.Path);

        var items = new List<RenameFileOperation>();

        foreach (var file in files)
        {
            var newPath = GetNewPath(settings, file);
            items.Add(new() { Path = file.Path, NewPath = newPath });
        }

        var index = 0;

        while (true)
        {
            console.Clear();

            index = CommandHelper.ClampIndex(index, files.Count);

            var item = items[index];

            CommandHelper.PrintCurrentFile(console, "Rename File", item.Path, index, files.Count);

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("Path", item.Path);
            grid.AddRow("New", item.NewPath);
            var panel = new Panel(grid);

            console.Write(panel);

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
            else if (cmd == NavCommand.Write)
            {
                fs.Move(item.Path, item.NewPath);
                item.IsSaved = true;
            }
            else
            {
                break;
            }
        }
        return 0;
    }

    private void WriteAll(List<RenameFileOperation> items)
    {
        foreach (var item in items.Where(x => !x.IsSaved).ToList())
        {
            fs.Move(item.Path, item.NewPath);
            item.IsSaved = true;
        }
    }

    private static string GetNewPath(RenameFileSettings settings, FileWithTagData file)
    {
        var dir = Path.GetDirectoryName(file.Path)!;
        var formatter = new TagDataFormatter(file.TagData, file.Path);
        var newName = formatter.Format(settings.Template);
        newName = CommandHelper.CleanFileName(newName);
        newName = $"{newName}{Path.GetExtension(file.Path)}";
        var newPath = Path.Combine(dir, newName);
        return newPath;
    }
}

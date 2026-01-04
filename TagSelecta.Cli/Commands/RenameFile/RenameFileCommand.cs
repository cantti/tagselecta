using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.IO;

namespace TagSelecta.Cli.Commands.RenameFile;

public class RenameFileCommand(
    IAnsiConsole console,
    IFileSystem fs,
    IAudioFileScanner audioFileScanner,
    IUserActionReader userActionReader
) : Command<RenameFileSettings>
{
    protected override int Execute(
        CommandContext context,
        RenameFileSettings settings,
        CancellationToken cancellationToken
    )
    {
        AltScreen.Enter();

        var files = audioFileScanner.ScanAndRead(settings.Path);

        var items = new List<RenameFileOperation>();

        foreach (var file in files)
        {
            var newPath = FileRenamer.GetNewPath(settings, file);
            items.Add(new() { Path = file.Path, NewPath = newPath });
        }

        var index = 0;

        while (true)
        {
            console.Clear();

            index = Math.Clamp(index, 0, files.Count - 1);

            var item = items[index];

            CommandHelper.PrintCurrentFile(console, item.Path, index, files.Count);

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("Path", item.Path);
            grid.AddRow("New", item.NewPath);
            var panel = new Panel(grid);

            console.Write(panel);

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
                WriteAll(items);
                break;
            }
            else if (cmd == UserAction.Write)
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
}

using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.IO;

namespace TagSelecta.Cli.Commands.RenameFile;

public class RenameFileCommand(
    IAnsiConsole console,
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

            index = CommandHelper.ClampIndex(index, files.Count);

            var item = items[index];

            CommandHelper.PrintCurrentFile(console, item.Path, index, files.Count);

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("Path", item.Path);
            grid.AddRow("New", item.NewPath);
            var panel = new Panel(grid);

            console.Write(panel);

            var cmd = CommandHelper.ReadNavigationCommand(console, true);
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
                WriteAll(items);
                break;
            }
            else if (cmd == UserInput.Write)
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

using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Read;

public class ReadCommand(IAnsiConsole console, IAudioFileScanner audioFileScanner, ITagger tagger)
    : Command<ReadSettings>
{
    public override int Execute(
        CommandContext context,
        ReadSettings settings,
        CancellationToken cancellationToken
    )
    {
        AltScreen.Enter();

        console.Cursor.Hide();

        var files = CommandHelper.ScanAndRead(console, audioFileScanner, tagger, settings.Path);

        var index = 0;

        while (true)
        {
            console.Clear();

            index = CommandHelper.ClampIndex(index, files.Count);

            var file = files[index];

            CommandHelper.PrintCurrentFile(console, file.Path, index, files.Count);

            TagDataPrinter.PrintTagData(console, file.TagData);
            var cmd = CommandHelper.ReadNavigationCommand(console, false);
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
        return 0;
    }
}

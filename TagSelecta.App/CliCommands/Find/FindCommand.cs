using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.App.IO;
using TagSelecta.Tagging;

namespace TagSelecta.App.CliCommands.Find;

public class FindCommand(IAnsiConsole console, IAudioFileScanner audioFileScanner)
    : Command<FindSettings>
{
    protected override int Execute(
        CommandContext context,
        FindSettings settings,
        CancellationToken ct
    )
    {
        var files = audioFileScanner.ScanAndRead(settings.Path);
        foreach (var file in files)
        {
            var formatter = new TagDataFormatter(file.TagData, file.Path);
            var shouldPrint =
                string.IsNullOrWhiteSpace(settings.Query)
                || (formatter.Format("{{ " + settings.Query + " }}") == "true");
            if (shouldPrint)
            {
                console.WriteLine(file.Path);
            }
        }
        return 0;
    }
}

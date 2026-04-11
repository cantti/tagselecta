using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Cli.Find;

public class FindCommand(IAnsiConsole console, IAudioFileScanner audioFileScanner)
    : Command<FindSettings>
{
    public override int Execute(CommandContext context, FindSettings settings, CancellationToken ct)
    {
        var files = audioFileScanner.SearchAndRead(settings.Path, ct);
        foreach (var file in files)
        {
            var formatter = new TagDataFormatter(file.TagData, file.Path);
            var shouldPrint =
                string.IsNullOrWhiteSpace(settings.Query)
                || formatter.Format(settings.Query) == "true";
            if (shouldPrint)
            {
                console.WriteLine(file.Path);
            }
        }

        return 0;
    }
}

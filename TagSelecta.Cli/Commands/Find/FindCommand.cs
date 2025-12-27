using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Find;

public class FindCommand(IAnsiConsole console, IAudioFileScanner audioFileScanner, ITagger tagger)
    : Command<FindSettings>
{
    public override int Execute(CommandContext context, FindSettings settings, CancellationToken ct)
    {
        var files = audioFileScanner.Scan(settings.Path, true);
        Parallel.ForEach(
            files,
            file =>
            {
                TagData? tagData = null;
                try
                {
                    tagData = tagger.ReadTags(file);
                }
                catch { }
                if (tagData is null)
                    return;

                var formatter = new TagDataFormatter(tagData, file);

                var shouldPrint =
                    string.IsNullOrWhiteSpace(settings.Query)
                    || (formatter.Format("{{ " + settings.Query + " }}") == "true");

                if (shouldPrint)
                {
                    console.WriteLine(file);
                }
            }
        );
        return 0;
    }
}

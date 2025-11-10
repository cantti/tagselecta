using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Commands.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class FindSettings : BaseSettings
{
    [CommandOption("--query|-q")]
    [Description("Find query")]
    public string Query { get; set; } = "";
}

public class FindCommand(IAnsiConsole console) : Command<FindSettings>
{
    public override int Execute(CommandContext context, FindSettings settings, CancellationToken ct)
    {
        var files = FileHelper.GetAllAudioFiles(settings.Path, true);
        Parallel.ForEach(
            files,
            file =>
            {
                TagData? tagData = null;
                try
                {
                    tagData = Tagger.ReadTags(file);
                }
                catch { }
                if (tagData is null)
                    return;

                var shouldPrint =
                    string.IsNullOrWhiteSpace(settings.Query)
                    || (TagDataFormatter.Format("{{ " + settings.Query + " }}", tagData) == "true");

                if (shouldPrint)
                {
                    console.WriteLine(file);
                }
            }
        );
        return 0;
    }
}

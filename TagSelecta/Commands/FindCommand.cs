using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Formatting;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class FindSettings : FileSettings
{
    [CommandOption("--query|-q")]
    [Description("Find query")]
    public string Query { get; set; } = "";
}

public class FindCommand(IAnsiConsole console) : Command<FindSettings>
{
    public override int Execute(CommandContext context, FindSettings settings)
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
                var result = Formatter.Format("{{ " + settings.Query + " }}", tagData);
                if (result == "true")
                {
                    console.WriteLine(file);
                }
            }
        );
        return 0;
    }
}

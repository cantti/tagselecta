using AudioTagCli.Misc;
using AudioTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AudioTagCli.Commands;

public class CleanSettings : CommandSettings
{
    [CommandOption("-p|--path", true)]
    public required string Path { get; set; }
}

public class CleanCommand : Command<CleanSettings>
{
    private static readonly HashSet<string> allowedExtended = ["label", "catalognumber"];

    public override int Execute(CommandContext context, CleanSettings settings)
    {
        var files = Helper.GetAllAudioFiles(settings.Path, true);
        foreach (var file in files)
        {
            AnsiConsole.MarkupLineInterpolated($"[blue]Current file: {file}[/]");
            try
            {
                var tags = Tagger.ReadTags(file);
                AnsiConsole.MarkupLine("[blue]Current tags[/]");
                AnsiConsole.WriteLine(tags.ToString());
                // tags.Extended = tags
                //     .Extended.Where(x => allowedExtended.Contains(x.Key.ToLower()))
                //     .ToDictionary();
                Tagger.WriteTags(file, tags);
                AnsiConsole.MarkupLine("[blue]Updated tags[/]");
                AnsiConsole.WriteLine(Tagger.ReadTags(file).ToString());
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]{ex.ToString()}[/]");
            }
            AnsiConsole.WriteLine();
        }
        return 0;
    }
}

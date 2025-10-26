using AudioTagCli.BaseCommands;
using AudioTagCli.Tagging;
using Spectre.Console;

namespace AudioTagCli.Commands;

public class CleanSettings : FileProcessingSettings { }

public class CleanCommand(IAnsiConsole console) : FileProcessingCommandBase<CleanSettings>(console)
{
    protected override async Task ProcessFileAsync(
        StatusContext ctx,
        CleanSettings settings,
        List<string> files,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        Tagger.RemoveTags(file);
        Tagger.WriteTags(file, tags);
        await Task.CompletedTask;
    }
}

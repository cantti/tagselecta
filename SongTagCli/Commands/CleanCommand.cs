using SongTagCli.BaseCommands;
using SongTagCli.Tagging;
using Spectre.Console;

namespace SongTagCli.Commands;

public class CleanSettings : FileProcessingSettings { }

public class CleanCommand(IAnsiConsole console) : FileProcessingCommandBase<CleanSettings>(console)
{
    protected override async Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        CleanSettings settings,
        List<string> files,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        Tagger.RemoveTags(file);
        Tagger.WriteTags(file, tags);
        return await Task.FromResult(new ProcessFileResult(ProcessFileResultStatus.Success));
    }
}

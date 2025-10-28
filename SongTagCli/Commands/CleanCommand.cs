using SongTagCli.BaseCommands;
using SongTagCli.Tagging;
using Spectre.Console;

namespace SongTagCli.Commands;

public class CleanSettings : FileProcessingSettings { }

public class CleanCommand(IAnsiConsole console) : FileProcessingCommandBase<CleanSettings>(console)
{
    protected override Task<ResultStatus> ProcessFileAsync(
        CleanSettings settings,
        List<string> files,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        Tagger.RemoveTags(file);
        Tagger.WriteTags(file, tags);
        PrintTagData(tags);
        return Task.FromResult(ResultStatus.Success);
    }
}

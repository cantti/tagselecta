using SongTagCli.BaseCommands;
using SongTagCli.Tagging;
using Spectre.Console;

namespace SongTagCli.Commands;

public class ReadSettings : FileProcessingSettings { }

public class ReadCommand(IAnsiConsole console) : FileProcessingCommandBase<ReadSettings>(console)
{
    protected override Task<ResultStatus> ProcessFileAsync(
        ReadSettings settings,
        List<string> files,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        PrintTagData(tags);
        if (Continue())
        {
            return Task.FromResult(ResultStatus.Success);
        }
        else
        {
            return Task.FromResult(ResultStatus.Success);
        }
    }
}

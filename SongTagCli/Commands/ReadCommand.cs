using SongTagCli.BaseCommands;
using Spectre.Console;

namespace SongTagCli.Commands;

public class ReadSettings : FileProcessingSettings { }

public class ReadCommand(IAnsiConsole console) : FileProcessingCommandBase<ReadSettings>(console)
{
    protected override async Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        ReadSettings settings,
        List<string> files,
        string file
    )
    {
        return await Task.FromResult(new ProcessFileResult(ProcessFileResultStatus.Success));
    }
}

using AudioTagCli.BaseCommands;
using Spectre.Console;

namespace AudioTagCli.Commands;

public class ReadSettings : FileProcessingSettings { }

public class ReadCommand(IAnsiConsole console) : FileProcessingCommandBase<ReadSettings>(console)
{
    protected override async Task ProcessFileAsync(
        StatusContext ctx,
        ReadSettings settings,
        List<string> files,
        string file
    )
    {
        await Task.CompletedTask;
    }
}

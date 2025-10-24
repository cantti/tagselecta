using AudioTagCli.BaseCommands;
using AudioTagCli.Misc;
using AudioTagCli.Tagging;
using Spectre.Console;

namespace AudioTagCli.Commands;

public class ReadSettings : FileProcessingSettings { }

public class ReadCommand(IAnsiConsole console) : FileProcessingCommandBase<ReadSettings>(console)
{
    protected override async Task ProcessFileAsync(
        StatusContext ctx,
        ReadSettings settings,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        Console.PrintTagData(tags);
        await Task.CompletedTask;
    }
}

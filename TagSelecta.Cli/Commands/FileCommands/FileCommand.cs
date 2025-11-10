using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands;

namespace TagSelecta.Cli.Commands.FileCommands;

public class FileCommand<TSettings>(FileAction<TSettings> action, IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        var files = CommandHelper.GetFiles(console, settings.Path);

        var actionContext = new FileActionContext<TSettings>(console)
        {
            Files = files,
            Settings = settings,
        };

        for (var currentFileIndex = 0; currentFileIndex < files.Count; currentFileIndex++)
        {
            var currentFile = files[currentFileIndex];

            CommandHelper.PrintCurrentFile(console, currentFile, currentFileIndex, files.Count);
            try
            {
                actionContext.SetCurrentFile(currentFile, currentFileIndex);
                await action.ProcessFileAsync(actionContext);
            }
            catch (Exception ex)
            {
                CommandHelper.PrintStatusError(console);
                console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                continue;
            }
            console.WriteLine();
        }
        console.MarkupLineInterpolated($"[green]Finished![/]");

        return 0;
    }
}

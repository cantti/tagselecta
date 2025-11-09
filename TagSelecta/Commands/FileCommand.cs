using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.FileActions;

namespace TagSelecta.Commands;

public class FileCommand<TAction, TSettings>(TAction action, IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : BaseSettings
    where TAction : IFileAction<TSettings>
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
                await action.ProcessFile(actionContext);
                CommandHelper.PrintStatusSuccess(console);
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

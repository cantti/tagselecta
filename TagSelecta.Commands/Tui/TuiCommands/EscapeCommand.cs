namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("escape")]
public class EscapeCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        foreach (var file in context.SelectedFiles)
        {
            file.IsSelected = false;
        }

        context.CommandHelpEnabled = false;
        context.KeymapHelpEnabled = false;

        return Task.CompletedTask;
    }
}

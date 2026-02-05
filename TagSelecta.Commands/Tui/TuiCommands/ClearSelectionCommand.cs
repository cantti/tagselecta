namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("clearselection")]
public class ClearSelectionCommand : ITuiCommand
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

        return Task.CompletedTask;
    }
}

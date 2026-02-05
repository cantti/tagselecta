namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("togglefilter")]
public class ToggleFilterCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.FilterEnabled = !context.FilterEnabled;
        context.FocusedFileIndex = 0;
        return Task.CompletedTask;
    }
}

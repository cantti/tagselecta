namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("toggletree")]
public class ToggleTreeCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.TreeEnabled = !context.TreeEnabled;
        return Task.CompletedTask;
    }
}

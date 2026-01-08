namespace TagSelecta.Cli.Tui.TuiCommands;

[TuiCommand("moveup")]
public class MoveUpCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request)
    {
        context.FocusedOperationIndex--;
        return Task.CompletedTask;
    }
}

namespace TagSelecta.Cli.Tui.TuiCommands;

[TuiCommand("movedown")]
public class MoveDownCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request)
    {
        context.FocusedOperationIndex++;
        return Task.CompletedTask;
    }
}

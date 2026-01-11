namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("movedown")]
public class MoveDownCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.FocusedOperationIndex++;
        return Task.CompletedTask;
    }
}

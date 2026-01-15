namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("movedown")]
public class MoveDownCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.FocusedOperationIndex = Math.Clamp(
            context.FocusedOperationIndex + 1,
            0,
            Math.Max(0, context.VisibleOperations.Count() - 1)
        );
        return Task.CompletedTask;
    }
}

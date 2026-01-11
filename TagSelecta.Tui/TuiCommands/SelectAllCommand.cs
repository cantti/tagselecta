namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("selectall")]
public class SelectAllCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        foreach (var operation in context.VisibleOperations)
        {
            operation.IsSelected = true;
        }
        return Task.CompletedTask;
    }
}

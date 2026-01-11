namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("clearselection")]
public class ClearSelectionCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        foreach (var operation in context.SelectedOperations)
        {
            operation.IsSelected = false;
        }
        return Task.CompletedTask;
    }
}

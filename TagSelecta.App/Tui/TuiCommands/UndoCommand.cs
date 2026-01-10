namespace TagSelecta.App.Tui.TuiCommands;

[TuiCommand("undo")]
public class UndoCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        foreach (var operation in context.Operations.Where(x => x.HasChanges).ToList())
        {
            operation.Undo();
        }
        return Task.CompletedTask;
    }
}

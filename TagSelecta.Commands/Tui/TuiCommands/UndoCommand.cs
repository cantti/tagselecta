namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("undo")]
public class UndoCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        foreach (var file in context.SelectedFiles.Where(x => x.HasChanges).ToList())
        {
            token.ThrowIfCancellationRequested();
            file.Undo();
        }
        return Task.CompletedTask;
    }
}

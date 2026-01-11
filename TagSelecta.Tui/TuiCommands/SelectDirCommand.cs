namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("selectdir")]
public class SelectDirCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        if (context.FocusedOperation is null)
        {
            return Task.CompletedTask;
        }
        var dir = Path.GetDirectoryName(context.FocusedOperation.CurrentPath);
        foreach (
            var operation in context
                .Operations.Where(x => Path.GetDirectoryName(x.CurrentPath) == dir)
                .ToList()
        )
        {
            operation.IsSelected = true;
        }
        return Task.CompletedTask;
    }
}

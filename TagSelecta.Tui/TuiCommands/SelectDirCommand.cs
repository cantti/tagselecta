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
        var dir = Path.GetDirectoryName(context.FocusedOperation.GetCurrentPath());
        foreach (
            var operation in context
                .Operations.Where(x => Path.GetDirectoryName(x.GetCurrentPath()) == dir)
                .ToList()
        )
        {
            operation.IsSelected = true;
        }
        return Task.CompletedTask;
    }
}

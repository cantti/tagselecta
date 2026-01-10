namespace TagSelecta.Cli.Tui.TuiCommands;

[TuiCommand("select")]
public class SelectCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        if (context.FocusedOperation is null)
        {
            return Task.CompletedTask;
        }
        context.FocusedOperation.IsSelected = !context.FocusedOperation.IsSelected;
        context.FocusedOperationIndex++;
        return Task.CompletedTask;
    }
}

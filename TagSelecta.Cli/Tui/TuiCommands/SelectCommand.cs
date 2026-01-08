namespace TagSelecta.Cli.Tui.TuiCommands;

[TuiCommand("select")]
public class SelectCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request)
    {
        var focusedOperation = context.Operations.ElementAtOrDefault(context.FocusedOperationIndex);
        if (focusedOperation is null)
        {
            return Task.CompletedTask;
        }
        focusedOperation.IsSelected = !focusedOperation.IsSelected;
        context.FocusedOperationIndex++;
        return Task.CompletedTask;
    }
}

namespace TagSelecta.App.Tui.TuiCommands;

[TuiCommand("togglefilter")]
public class ToggleFilterCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.FilterEnabled = !context.FilterEnabled;
        context.FocusedOperationIndex = 0;
        return Task.CompletedTask;
    }
}

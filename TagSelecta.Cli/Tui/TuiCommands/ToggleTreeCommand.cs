namespace TagSelecta.Cli.Tui.TuiCommands;

[TuiCommand("toggletree")]
public class ToggleTreeCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.TreeEnabled = !context.TreeEnabled;
        return Task.CompletedTask;
    }
}

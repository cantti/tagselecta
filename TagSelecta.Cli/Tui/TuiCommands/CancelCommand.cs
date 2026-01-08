namespace TagSelecta.Cli.Tui.TuiCommands;

[TuiCommand("cancel")]
public class CancelCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.Print("Cancelled.");
        return Task.CompletedTask;
    }
}

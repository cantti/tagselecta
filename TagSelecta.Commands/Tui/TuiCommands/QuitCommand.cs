namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("quit", "q")]
public class QuitCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.Quit();
        return Task.CompletedTask;
    }
}

namespace TagSelecta.Cli.Tui.TuiCommands;

public class QuitCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request)
    {
        // context.Quit();
        return Task.CompletedTask;
    }
}

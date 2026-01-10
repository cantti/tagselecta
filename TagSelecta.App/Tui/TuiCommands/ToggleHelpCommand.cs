namespace TagSelecta.App.Tui.TuiCommands;

[TuiCommand("togglehelp")]
public class ToggleHelpCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.HelpEnabled = !context.HelpEnabled;
        return Task.CompletedTask;
    }
}

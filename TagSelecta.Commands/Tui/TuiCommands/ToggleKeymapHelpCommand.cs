namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("togglekeymaphelp")]
public class ToggleKeymapHelpCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.CommandHelpEnabled = false;
        context.KeymapHelpEnabled = !context.KeymapHelpEnabled;
        return Task.CompletedTask;
    }
}

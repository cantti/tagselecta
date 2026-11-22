namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("escape")]
public class EscapeCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        if (context.CommandHelpEnabled || context.PictureEnabled)
        {
            context.CommandHelpEnabled = false;
            context.HelpTopic = "";
            context.PictureEnabled = false;
        }
        else
        {
            foreach (var file in context.SelectedFiles)
            {
                file.IsSelected = false;
            }
        }

        return Task.CompletedTask;
    }
}

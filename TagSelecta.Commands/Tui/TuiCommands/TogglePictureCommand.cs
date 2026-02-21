namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("togglepicture")]
public class TogglePictureCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.PictureEnabled = !context.PictureEnabled;
        return Task.CompletedTask;
    }
}

namespace TagSelecta.Tui.TuiCommands;

public interface ITuiCommand
{
    Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token);
}

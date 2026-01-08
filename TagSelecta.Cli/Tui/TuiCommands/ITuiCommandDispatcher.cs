namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommandDispatcher
{
    Task DispatchAsync(
        ITuiCommand command,
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    );
}

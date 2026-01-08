namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommandDispatcher
{
    Task DispatchAsync(ITuiCommandContext context, Request request, CancellationToken token);
}

namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommand
{
    bool BlockInput => false;

    Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token);
}

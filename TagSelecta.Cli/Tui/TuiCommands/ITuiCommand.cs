namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommand
{
    Task ExecuteAsync(ITuiCommandContext context, Request request);
}

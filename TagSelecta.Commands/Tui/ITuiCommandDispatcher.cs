using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public interface ITuiCommandDispatcher
{
    Task<bool> Execute(string commandText, ITuiCommandContext context, CancellationToken token);
}

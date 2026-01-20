namespace TagSelecta.Commands.Tui.TuiCommands;

public interface ITuiCommandFactory
{
    ITuiCommand Create(string name);
}

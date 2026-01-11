namespace TagSelecta.Tui.TuiCommands;

public interface ITuiCommandFactory
{
    ITuiCommand? Create(string name);
}

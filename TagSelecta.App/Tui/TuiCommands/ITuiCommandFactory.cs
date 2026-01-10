namespace TagSelecta.App.Tui.TuiCommands;

public interface ITuiCommandFactory
{
    ITuiCommand? Create(string name);
}

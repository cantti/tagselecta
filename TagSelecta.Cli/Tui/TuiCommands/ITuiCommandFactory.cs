namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommandFactory
{
    ITuiCommand? Create(string name);
}

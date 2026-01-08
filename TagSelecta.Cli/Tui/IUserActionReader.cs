using System.Text;
using TagSelecta.Cli.Tui.TuiCommands;

namespace TagSelecta.Cli.Tui;

public interface IUserActionReader
{
    Request? Read(ConsoleKeyInfo key);
    StringBuilder Buffer { get; }
    int Cursor { get; }
    InputMode Mode { get; }
}

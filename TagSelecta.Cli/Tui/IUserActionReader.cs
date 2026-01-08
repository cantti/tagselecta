using System.Text;
using TagSelecta.Cli.Tui.TuiCommands;

namespace TagSelecta.Cli.Tui;

public interface IUserActionReader
{
    bool TryRead(ConsoleKeyInfo key, out Request request);
    StringBuilder Buffer { get; }
    int Cursor { get; }
    InputMode Mode { get; }
}

using System.Text;
using TagSelecta.Cli.Tui.TuiCommands;

namespace TagSelecta.Cli.Tui;

public interface IUserActionReader
{
    bool TryRead(ConsoleKeyInfo key, out Request request);
    int CursorPos { get; }
    InputMode Mode { get; }
    string Text { get; }
}

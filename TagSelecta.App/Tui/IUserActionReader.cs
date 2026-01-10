using System.Text;
using TagSelecta.App.Tui.TuiCommands;

namespace TagSelecta.App.Tui;

public interface IUserActionReader
{
    bool TryRead(ConsoleKeyInfo key, out Request request);
    int CursorPos { get; }
    InputMode Mode { get; }
    string Text { get; }
}

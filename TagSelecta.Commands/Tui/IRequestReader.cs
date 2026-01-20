using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public interface IRequestReader
{
    bool TryRead(ConsoleKeyInfo key, out Request request);
    int CursorPos { get; }
    InputMode Mode { get; }
    string Text { get; }
}

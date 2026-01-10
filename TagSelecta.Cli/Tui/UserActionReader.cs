using System.Text;
using Spectre.Console;
using TagSelecta.Cli.Tui.TuiCommands;

namespace TagSelecta.Cli.Tui;

public class UserActionReader : IUserActionReader
{
    private readonly HotkeyMap _hotkeys;
    private readonly CommandParser _parser;

    private readonly StringBuilder _buffer = new();

    public UserActionReader(HotkeyMap hotkeys, CommandParser parser)
    {
        _hotkeys = hotkeys;
        _parser = parser;
    }

    public bool TryRead(ConsoleKeyInfo key, out Request request)
    {
        var handleResult =
            Mode == InputMode.Normal ? HandleNormalMode(key) : HandleCommandMode(key);
        if (handleResult != null)
        {
            request = handleResult;
            return true;
        }
        request = new Request("", []);
        return false;
    }

    private Request? HandleNormalMode(ConsoleKeyInfo key)
    {
        if (key.KeyChar == ':')
        {
            Mode = InputMode.Command;
            _buffer.Clear();
            CursorPos = 0;
            return null;
        }

        var action = _hotkeys.Resolve(key);
        return action != null ? new Request(action, []) : null;
    }

    private Request? HandleCommandMode(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Escape:
                ExitCommandMode();
                return null;

            case ConsoleKey.Enter:
                var text = _buffer.ToString();
                ExitCommandMode();
                return _parser.TryParse(text, out var request) ? request : default;

            case ConsoleKey.LeftArrow:
                if (CursorPos > 0)
                    CursorPos--;
                return null;

            case ConsoleKey.RightArrow:
                if (CursorPos < _buffer.Length)
                    CursorPos++;
                return null;

            case ConsoleKey.Home:
                CursorPos = 0;
                return null;

            case ConsoleKey.End:
                CursorPos = _buffer.Length;
                return null;

            case ConsoleKey.Backspace:
                if (CursorPos > 0)
                {
                    _buffer.Remove(CursorPos - 1, 1);
                    CursorPos--;
                }
                return null;

            case ConsoleKey.Delete:
                if (CursorPos < _buffer.Length)
                {
                    _buffer.Remove(CursorPos, 1);
                }
                return null;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _buffer.Insert(CursorPos, key.KeyChar);
            CursorPos++;
        }

        return null;
    }

    private void ExitCommandMode()
    {
        Mode = InputMode.Normal;
        _buffer.Clear();
        CursorPos = 0;
    }

    public string Text => _buffer.ToString();

    public int CursorPos { get; private set; }

    public InputMode Mode { get; private set; } = InputMode.Normal;
}

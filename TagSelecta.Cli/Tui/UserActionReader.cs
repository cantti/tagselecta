using System.Text;
using Spectre.Console;

namespace TagSelecta.Cli.Tui;

public class UserActionReader : IUserActionReader
{
    private readonly HotkeyMap _hotkeys;
    private readonly CommandParser _parser;

    private InputMode _mode = InputMode.Normal;

    private readonly StringBuilder _buffer = new();
    private int _cursor = 0;

    // ANSI color codes
    private const string CmdStyle = "\x1b[30;43m"; // black on yellow
    private const string ResetStyle = "\x1b[0m";

    public UserActionReader(HotkeyMap hotkeys, CommandParser parser)
    {
        _hotkeys = hotkeys;
        _parser = parser;
    }

    public ActionRequest? Read(ConsoleKeyInfo key)
    {
        if (_mode == InputMode.Normal)
            return HandleNormalMode(key);
        return HandleCommandMode(key);
    }

    private ActionRequest? HandleNormalMode(ConsoleKeyInfo key)
    {
        if (key.KeyChar == ':')
        {
            _mode = InputMode.Command;
            _buffer.Clear();
            _cursor = 0;
            return null;
        }

        var action = _hotkeys.Resolve(key);
        return action != null ? new ActionRequest(action, []) : null;
    }

    private ActionRequest? HandleCommandMode(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Escape:
                ExitCommandMode();
                return null;

            case ConsoleKey.Enter:
                var text = _buffer.ToString();
                ExitCommandMode();
                return _parser.TryParse(text, out var request) ? request : null;

            case ConsoleKey.LeftArrow:
                if (_cursor > 0)
                    _cursor--;
                return null;

            case ConsoleKey.RightArrow:
                if (_cursor < _buffer.Length)
                    _cursor++;
                return null;

            case ConsoleKey.Home:
                _cursor = 0;
                return null;

            case ConsoleKey.End:
                _cursor = _buffer.Length;
                return null;

            case ConsoleKey.Backspace:
                if (_cursor > 0)
                {
                    _buffer.Remove(_cursor - 1, 1);
                    _cursor--;
                }
                return null;

            case ConsoleKey.Delete:
                if (_cursor < _buffer.Length)
                {
                    _buffer.Remove(_cursor, 1);
                }
                return null;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _buffer.Insert(_cursor, key.KeyChar);
            _cursor++;
        }

        return null;
    }

    private void ExitCommandMode()
    {
        _mode = InputMode.Normal;
        _buffer.Clear();
        _cursor = 0;
    }

    public StringBuilder Buffer => _buffer;

    public int Cursor => _cursor;

    public InputMode Mode => _mode;
}

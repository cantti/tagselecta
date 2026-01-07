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

    public UserActionReader(HotkeyMap hotkeys, CommandParser parser)
    {
        _hotkeys = hotkeys;
        _parser = parser;
    }

    public ActionRequest Read()
    {
        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (_mode == InputMode.Normal)
            {
                var evt = HandleNormalMode(key);
                if (evt != null)
                    return evt;
            }
            else
            {
                var evt = HandleCommandMode(key);
                if (evt != null)
                    return evt;
            }
        }
    }

    private ActionRequest? HandleNormalMode(ConsoleKeyInfo key)
    {
        // Enter command mode
        if (key.KeyChar == ':')
        {
            _mode = InputMode.Command;
            _buffer.Clear();
            _cursor = 0;
            RenderCommandPrompt();
            return null;
        }

        // Resolve hotkey
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
                UpdateCursor();
                return null;

            case ConsoleKey.RightArrow:
                if (_cursor < _buffer.Length)
                    _cursor++;
                UpdateCursor();
                return null;

            case ConsoleKey.Home:
                _cursor = 0;
                UpdateCursor();
                return null;

            case ConsoleKey.End:
                _cursor = _buffer.Length;
                UpdateCursor();
                return null;

            case ConsoleKey.Backspace:
                if (_cursor > 0)
                {
                    _buffer.Remove(_cursor - 1, 1);
                    _cursor--;
                    RedrawCommandLine();
                }
                return null;

            case ConsoleKey.Delete:
                if (_cursor < _buffer.Length)
                {
                    _buffer.Remove(_cursor, 1);
                    RedrawCommandLine();
                }
                return null;
        }

        // Insert printable characters at cursor
        if (!char.IsControl(key.KeyChar))
        {
            _buffer.Insert(_cursor, key.KeyChar);
            _cursor++;
            RedrawCommandLine();
        }

        return null;
    }

    private void RenderCommandPrompt()
    {
        AnsiConsole.Cursor.Show();
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(":");
        UpdateCursor();
    }

    private void RedrawCommandLine()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(":" + _buffer + " ");

        UpdateCursor();
    }

    private void UpdateCursor()
    {
        Console.SetCursorPosition(1 + _cursor, Console.WindowHeight - 1);
    }

    private void ExitCommandMode()
    {
        AnsiConsole.Cursor.Show();
        _mode = InputMode.Normal;
        _buffer.Clear();
        _cursor = 0;
        ClearCommandLine();
    }

    private void ClearCommandLine()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(new string(' ', Console.WindowWidth));
    }
}

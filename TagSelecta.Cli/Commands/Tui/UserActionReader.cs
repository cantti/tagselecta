using System.Text;

namespace TagSelecta.Cli.Commands.Tui;

public class UserActionReader : IUserActionReader
{
    private readonly HotkeyMap _hotkeys;
    private readonly CommandParser _parser;

    private InputMode _mode = InputMode.Normal;
    private readonly StringBuilder _buffer = new();

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
            RenderCommandPrompt();
            return null;
        }

        // Resolve hotkey
        var action = _hotkeys.Resolve(key);
        return action != null ? new ActionRequest(action, []) : null;
    }

    private ActionRequest? HandleCommandMode(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            ExitCommandMode();
            return null;
        }

        if (key.Key == ConsoleKey.Enter)
        {
            var text = _buffer.ToString();
            ExitCommandMode();

            return _parser.TryParse(text, out var request) ? request : null;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (_buffer.Length > 0)
            {
                _buffer.Length--;
                RedrawCommandLine();
            }
            return null;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _buffer.Append(key.KeyChar);
            RedrawCommandLine();
        }

        return null;
    }

    private void ExitCommandMode()
    {
        _mode = InputMode.Normal;
        _buffer.Clear();
        ClearCommandLine();
    }

    private void RenderCommandPrompt()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(":");
    }

    private void RedrawCommandLine()
    {
        Console.SetCursorPosition(1, Console.WindowHeight - 1);
        Console.Write(_buffer.ToString() + " ");
        Console.SetCursorPosition(1 + _buffer.Length, Console.WindowHeight - 1);
    }

    private void ClearCommandLine()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(new string(' ', Console.WindowWidth));
    }
}

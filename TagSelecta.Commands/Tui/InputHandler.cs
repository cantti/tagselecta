using System.Text;

namespace TagSelecta.Commands.Tui;

public class InputHandler(HotkeyMap hotkeys)
{
    private readonly StringBuilder _buffer = new();
    private readonly List<string> _history = [];

    // -1 = not navigating history
    private int _historyIndex = -1;

    public string Text => _buffer.ToString();
    public int CursorPos { get; private set; }
    public InputMode Mode { get; private set; } = InputMode.Normal;

    public bool ProcessKey(ConsoleKeyInfo key, out string commandText)
    {
        var handleResult =
            Mode == InputMode.Normal ? HandleNormalMode(key) : HandleCommandMode(key);

        if (handleResult != null)
        {
            commandText = handleResult;
            return true;
        }

        commandText = "";
        return false;
    }

    public void SetText(string text)
    {
        Mode = InputMode.Command;
        _buffer.Clear();
        _buffer.Append(text);
        CursorPos = _buffer.Length;
    }

    private string? HandleNormalMode(ConsoleKeyInfo key)
    {
        if (key.KeyChar == ':')
        {
            Mode = InputMode.Command;
            _buffer.Clear();
            CursorPos = 0;
            return null;
        }

        return hotkeys.Resolve(key);
    }

    private string? HandleCommandMode(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Escape:
                ExitCommandMode(false);
                return null;

            case ConsoleKey.Enter:
                var text = _buffer.ToString();
                ExitCommandMode(true, text);
                return text;

            case ConsoleKey.LeftArrow:
                if (CursorPos > 0)
                {
                    CursorPos--;
                }

                return null;

            case ConsoleKey.RightArrow:
                if (CursorPos < _buffer.Length)
                {
                    CursorPos++;
                }

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

            case ConsoleKey.UpArrow:
                NavigateHistoryUp();
                return null;

            case ConsoleKey.DownArrow:
                NavigateHistoryDown();
                return null;
        }

        // typing resets history navigation
        if (!char.IsControl(key.KeyChar))
        {
            _historyIndex = -1;
            _buffer.Insert(CursorPos, key.KeyChar);
            CursorPos++;
        }

        return null;
    }

    private void NavigateHistoryUp()
    {
        if (_history.Count == 0)
        {
            return;
        }

        if (_historyIndex == -1)
        {
            _historyIndex = _history.Count - 1;
        }
        else if (_historyIndex > 0)
        {
            _historyIndex--;
        }

        LoadHistoryEntry();
    }

    private void NavigateHistoryDown()
    {
        if (_historyIndex == -1)
        {
            return;
        }

        if (_historyIndex < _history.Count - 1)
        {
            _historyIndex++;
            LoadHistoryEntry();
        }
        else
        {
            // past newest => empty input
            _historyIndex = -1;
            _buffer.Clear();
            CursorPos = 0;
        }
    }

    private void LoadHistoryEntry()
    {
        _buffer.Clear();
        _buffer.Append(_history[_historyIndex]);
        CursorPos = _buffer.Length;
    }

    private void ExitCommandMode(bool addToHistory, string? text = null)
    {
        Mode = InputMode.Normal;

        if (addToHistory && !string.IsNullOrWhiteSpace(text))
        {
            _history.Add(text);
        }

        _historyIndex = -1;
        _buffer.Clear();
        CursorPos = 0;
    }
}

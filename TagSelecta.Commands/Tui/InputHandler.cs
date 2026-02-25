namespace TagSelecta.Commands.Tui;

public class InputHandler(HotkeyMap hotkeys, ICompletionProvider completionProvider)
{
    private readonly List<string> _history = [];
    private int _completionIndex;
    private List<string> _completions = [];

    // -1 = not navigating history
    private int _historyIndex = -1;

    public string Input { get; private set; } = "";

    public string Completion => _completions.Count > 0 ? _completions[_completionIndex] : "";

    public int CursorPos { get; private set; }
    public InputMode Mode { get; private set; } = InputMode.Normal;

    public bool IsAutoCompletionEnabled { get; set; } = true;

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
        Input = text;
        CursorPos = Input.Length;
    }

    private string? HandleNormalMode(ConsoleKeyInfo key)
    {
        if (key.KeyChar == ':')
        {
            Mode = InputMode.Command;
            Input = "";
            CursorPos = 0;
            return null;
        }

        return hotkeys.Resolve(key);
    }

    private string? HandleCommandMode(ConsoleKeyInfo key)
    {
        // ctrl+space => complete or cycle through completions
        if (
            key.Modifiers.HasFlag(ConsoleModifiers.Control)
            && (key.Key == ConsoleKey.Spacebar || key.KeyChar == '\0')
        )
        {
            if (_completions.Count == 0)
            {
                LoadCompletions();
            }
            else
            {
                _completionIndex = (_completionIndex + 1) % _completions.Count;
            }

            return null;
        }

        // keys other than tab => reset completion
        if (key.Key != ConsoleKey.Tab)
        {
            ClearCompletions();
        }

        if (key.Key == ConsoleKey.Escape)
        {
            ExitCommandMode(false);
            return null;
        }

        if (key.Key == ConsoleKey.Enter)
        {
            var text = Input;
            ExitCommandMode(true);
            return text;
        }

        if (key.Key == ConsoleKey.LeftArrow)
        {
            if (CursorPos > 0)
            {
                CursorPos--;
            }

            return null;
        }

        if (key.Key == ConsoleKey.RightArrow)
        {
            if (CursorPos < Input.Length)
            {
                CursorPos++;
            }

            return null;
        }

        if (key.Key == ConsoleKey.Home)
        {
            CursorPos = 0;
            return null;
        }

        if (key.Key == ConsoleKey.End)
        {
            CursorPos = Input.Length;
            return null;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (CursorPos > 0)
            {
                Input = Input.Remove(CursorPos - 1, 1);
                CursorPos--;
            }

            return null;
        }

        if (key.Key == ConsoleKey.Delete)
        {
            if (CursorPos < Input.Length)
            {
                Input = Input.Remove(CursorPos, 1);
            }

            ClearCompletions();
            return null;
        }

        if (key.Key == ConsoleKey.UpArrow)
        {
            NavigateHistoryUp();
            return null;
        }

        if (key.Key == ConsoleKey.DownArrow)
        {
            NavigateHistoryDown();
            return null;
        }

        if (key.Key == ConsoleKey.Tab)
        {
            Input = Input.Insert(CursorPos, Completion);
            CursorPos += Completion.Length;
            ClearCompletions();
            return null;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _historyIndex = -1;
            Input = Input.Insert(CursorPos, key.KeyChar.ToString());
            CursorPos++;
            // autocomplete
            if (IsAutoCompletionEnabled && key.KeyChar != ' ')
            {
                LoadCompletions();
            }
        }

        return null;
    }

    private void ClearCompletions()
    {
        _completionIndex = 0;
        _completions = [];
    }

    private void LoadCompletions()
    {
        _completions = completionProvider.GetCompletions(Input, CursorPos).ToList();
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
            Input = "";
            CursorPos = 0;
        }
    }

    private void LoadHistoryEntry()
    {
        Input = _history[_historyIndex];
        CursorPos = Input.Length;
    }

    private void ExitCommandMode(bool addToHistory)
    {
        Mode = InputMode.Normal;

        if (addToHistory && !string.IsNullOrWhiteSpace(Input))
        {
            _history.Add(Input);
        }

        _historyIndex = -1;
        Input = "";
        CursorPos = 0;
        ClearCompletions();
    }
}

namespace TagSelecta.Commands.Tui;

public class InputHandler(HotkeyMap hotkeys, ICompletionProvider completionProvider)
{
    private readonly List<string> _history = [];

    // -1 = not navigating history
    private int _historyIndex = -1;

    public string Input { get; private set; } = "";

    public string Completion { get; private set; } = "";

    public int CompletionIndex { get; set; }

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
        // ctrl+space => complete
        if (
            key.Modifiers.HasFlag(ConsoleModifiers.Control)
            && (key.Key == ConsoleKey.Spacebar || key.KeyChar == '\0')
        )
        {
            CompletionIndex++;
            Completion = completionProvider.GetCompletion(Input, CursorPos, CompletionIndex);
            return null;
        }

        switch (key.Key)
        {
            case ConsoleKey.Escape:
                ExitCommandMode(false);
                return null;

            case ConsoleKey.Enter:
                var text = Input;
                ExitCommandMode(true);
                return text;

            case ConsoleKey.LeftArrow:
                if (CursorPos > 0)
                {
                    CursorPos--;
                }

                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.RightArrow:
                if (CursorPos < Input.Length)
                {
                    CursorPos++;
                }

                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.Home:
                CursorPos = 0;
                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.End:
                CursorPos = Input.Length;
                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.Backspace:
                if (CursorPos > 0)
                {
                    Input = Input.Remove(CursorPos - 1, 1);
                    CursorPos--;
                }

                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.Delete:
                if (CursorPos < Input.Length)
                {
                    Input = Input.Remove(CursorPos, 1);
                }

                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.UpArrow:
                NavigateHistoryUp();
                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.DownArrow:
                NavigateHistoryDown();
                Completion = "";
                CompletionIndex = 0;
                return null;

            case ConsoleKey.Tab:
                Input = Input.Insert(CursorPos, Completion);
                CursorPos += Completion.Length;
                Completion = "";
                return null;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _historyIndex = -1;
            Input = Input.Insert(CursorPos, key.KeyChar.ToString());
            CursorPos++;
            // do not show completion if typing space
            if (key.KeyChar != ' ')
            {
                Completion = completionProvider.GetCompletion(Input, CursorPos, 0);
            }
            else
            {
                Completion = "";
            }
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
        Completion = "";
    }
}

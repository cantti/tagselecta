namespace TagSelecta.Commands.Tui;

public sealed class HotkeyMap
{
    private readonly Dictionary<string, string> _map = new(StringComparer.Ordinal);

    public void Bind(string key, string actionName) => _map[NormalizeKey(key)] = actionName;

    public string? Resolve(ConsoleKeyInfo key)
    {
        var token = ToToken(key);
        if (token is not null && _map.TryGetValue(NormalizeKey(token), out var byToken))
        {
            return byToken;
        }

        if (key.KeyChar != '\0')
        {
            var s = NormalizeKey(key.KeyChar.ToString());
            if (_map.TryGetValue(s, out var byChar))
            {
                return byChar;
            }
        }

        return null;
    }

    private static string NormalizeKey(string key)
    {
        var trimmed = key.Trim();
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("Hotkey cannot be empty.", nameof(key));
        }
        return trimmed.Length == 1 ? trimmed : trimmed.ToLowerInvariant();
    }

    private static string? ToToken(ConsoleKeyInfo key) =>
        key.Key switch
        {
            ConsoleKey.Escape => "esc",
            ConsoleKey.UpArrow => "up",
            ConsoleKey.DownArrow => "down",
            ConsoleKey.LeftArrow => "left",
            ConsoleKey.RightArrow => "right",
            ConsoleKey.Home => "home",
            ConsoleKey.End => "end",
            ConsoleKey.PageUp => "pgup",
            ConsoleKey.PageDown => "pgdn",
            ConsoleKey.Enter => "enter",
            ConsoleKey.Spacebar => "space",
            ConsoleKey.Tab => "tab",
            ConsoleKey.Backspace => "backspace",
            ConsoleKey.Delete => "delete",
            _ => null,
        };
}

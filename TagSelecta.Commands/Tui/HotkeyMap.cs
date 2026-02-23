namespace TagSelecta.Commands.Tui;

public sealed class HotkeyMap
{
    private readonly Dictionary<string, string> _map = new(StringComparer.Ordinal);

    public void Bind(string key, string actionName)
    {
        _map[NormalizeKey(key)] = actionName;
    }

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
        return trimmed.Length == 1 ? trimmed : trimmed.ToLowerInvariant();
    }

    private static string? ToToken(ConsoleKeyInfo key)
    {
        return key.Key switch
        {
            ConsoleKey.Escape => HotkeyTokens.Esc,
            ConsoleKey.UpArrow => HotkeyTokens.Up,
            ConsoleKey.DownArrow => HotkeyTokens.Down,
            ConsoleKey.LeftArrow => HotkeyTokens.Left,
            ConsoleKey.RightArrow => HotkeyTokens.Right,
            ConsoleKey.Home => HotkeyTokens.Home,
            ConsoleKey.End => HotkeyTokens.End,
            ConsoleKey.PageUp => HotkeyTokens.PageUp,
            ConsoleKey.PageDown => HotkeyTokens.PageDown,
            ConsoleKey.Enter => HotkeyTokens.Enter,
            ConsoleKey.Spacebar => HotkeyTokens.Space,
            ConsoleKey.Tab => HotkeyTokens.Tab,
            ConsoleKey.Backspace => HotkeyTokens.Backspace,
            ConsoleKey.Delete => HotkeyTokens.Delete,
            _ => null,
        };
    }
}

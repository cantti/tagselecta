namespace TagSelecta.Cli.Commands.Tui;

public sealed class HotkeyMap
{
    private readonly Dictionary<ConsoleKey, string> _map = new();

    public void Bind(ConsoleKey key, string actionName) => _map[key] = actionName;

    public string? Resolve(ConsoleKeyInfo key) =>
        _map.TryGetValue(key.Key, out var action) ? action : null;
}

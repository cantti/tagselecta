namespace TagSelecta.Commands.Tui.TuiCommands;

public class ParsedCommandOption(string key, string value)
{
    public string Key { get; } = key;
    public string Value { get; } = value;
}

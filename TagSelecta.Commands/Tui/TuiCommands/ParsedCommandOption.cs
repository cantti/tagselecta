namespace TagSelecta.Commands.Tui.TuiCommands;

public class ParsedCommandOption
{
    public ParsedCommandOption(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string Value { get; }
}

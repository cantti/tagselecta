namespace TagSelecta.Commands.Tui.TuiCommands;

public class ParsedCommand(string name, IEnumerable<ParsedCommandOption> options)
{
    public string Name { get; } = name;
    public IReadOnlyCollection<ParsedCommandOption> Options { get; } = options.ToList();
}

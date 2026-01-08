using System.Reflection;

namespace TagSelecta.Cli.Tui.TuiCommands;

public class TuiCommandFactory : ITuiCommandFactory
{
    private readonly List<(string[] Names, ITuiCommand command)> _commands = [];

    public TuiCommandFactory(IEnumerable<ITuiCommand> commands)
    {
        foreach (var command in commands)
        {
            var type = command.GetType();
            var attr = type.GetCustomAttribute<TuiCommandAttribute>();
            if (attr is null || attr.Names.Length == 0)
                continue;

            _commands.Add((attr.Names, command));
        }
    }

    public ITuiCommand? Create(string name)
    {
        var command = _commands.FirstOrDefault(c => c.Names.Contains(name));
        if (command == default)
        {
            command = _commands.SingleOrDefault(c => c.command.GetType() == typeof(TagDataCommand));
        }
        return command.command;
    }
}

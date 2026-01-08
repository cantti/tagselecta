using System.Reflection;

namespace TagSelecta.Cli.Tui.TuiCommands;

public class TuiCommandDispatcher : ITuiCommandDispatcher
{
    private readonly List<(string[] Names, ITuiCommand command)> _commands = [];

    public TuiCommandDispatcher(IEnumerable<ITuiCommand> commands)
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

    public async Task DispatchAsync(ITuiCommandContext context, Request request)
    {
        var command = _commands.FirstOrDefault(c => c.Names.Contains(request.Name));
        if (command == default)
        {
            command = _commands.FirstOrDefault(c => c.GetType() == typeof(TagDataCommand));
        }
        if (command != default)
        {
            await command.command.ExecuteAsync(context, request);
        }
    }
}

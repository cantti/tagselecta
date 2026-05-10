using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.Commands.Tui.TuiCommands;

public class TuiCommandFactory : ITuiCommandFactory
{
    private readonly List<TuiCommandDescriptor> _commands = [];

    public TuiCommandFactory(IServiceProvider provider)
    {
        var commands = provider.GetServices<ITuiCommand>();
        foreach (var command in commands)
        {
            var type = command.GetType();
            var attr = type.GetCustomAttribute<TuiCommandAttribute>();
            if (attr is null || attr.Names.Length == 0)
            {
                continue;
            }

            _commands.Add(
                new TuiCommandDescriptor(
                    attr.Names,
                    () => provider.GetServices<ITuiCommand>().Single(x => x.GetType() == type),
                    command.GetType()
                )
            );
        }
    }

    public ITuiCommand Create(string name)
    {
        var command =
            _commands.SingleOrDefault(c => c.Names.Contains(name))
            ?? _commands.Single(c => c.Type == typeof(ExecuteTagDataActionCommand));
        return command.Command.Invoke();
    }

    private class TuiCommandDescriptor(
        IEnumerable<string> names,
        Func<ITuiCommand> command,
        Type type
    )
    {
        public IReadOnlyList<string> Names { get; } = names.ToList();
        public Func<ITuiCommand> Command { get; } = command;
        public Type Type { get; } = type;
    }
}

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.Commands.Tui.TuiCommands;

public class TuiCommandFactory : ITuiCommandFactory
{
    private readonly List<TuiCommandDescriptor> _commands = [];
    private readonly IServiceProvider _provider;

    public TuiCommandFactory(IServiceProvider provider)
    {
        _provider = provider;

        var commandTypes = typeof(ITuiCommand)
            .Assembly.GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false }
                && typeof(ITuiCommand).IsAssignableFrom(type)
                && type.GetCustomAttribute<TuiCommandAttribute>() is not null
            )
            .Select(type => new
            {
                Type = type,
                Attribute = type.GetCustomAttribute<TuiCommandAttribute>()!,
            })
            .ToList();

        foreach (var entry in commandTypes)
        {
            _commands.Add(
                new TuiCommandDescriptor(
                    entry.Attribute.Names,
                    () =>
                        provider.GetServices<ITuiCommand>().Single(x => x.GetType() == entry.Type),
                    entry.Type
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

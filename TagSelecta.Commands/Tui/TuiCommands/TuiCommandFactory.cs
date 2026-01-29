using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.Commands.Tui.TuiCommands;

public class TuiCommandFactory : ITuiCommandFactory
{
    private readonly IServiceProvider _provider;
    private readonly List<(string[] Names, ITuiCommand command)> _commands = [];

    public TuiCommandFactory(IServiceProvider provider)
    {
        _provider = provider;
        var commands = provider.GetServices<ITuiCommand>();
        commands = commands.Append(CreateMacroCommand());
        foreach (var command in commands)
        {
            var type = command.GetType();
            var attr = type.GetCustomAttribute<TuiCommandAttribute>();
            if (attr is null || attr.Names.Length == 0)
                continue;

            _commands.Add((attr.Names, command));
        }
    }

    public ITuiCommand Create(string name)
    {
        var command = _commands.SingleOrDefault(c => c.Names.Contains(name));
        if (command == default)
        {
            command = _commands.Single(c =>
                c.command.GetType() == typeof(ExecuteTagDataActionCommand)
            );
        }
        return command.command;
    }

    private ITuiCommand CreateMacroCommand()
    {
        var commandParser = _provider.GetService<CommandParser>()!;
        var macroSettings = _provider.GetService<MacroSettings>()!;
        return new MacroCommand(this, commandParser, macroSettings);
    }
}

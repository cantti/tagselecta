using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Commands.Tui.TuiCommands;

public class MacroSettings
{
    public Dictionary<string, string> Macros { get; set; } = new();
}

[TuiCommand("macro", "m")]
public class MacroCommand(
    ITuiCommandFactory commandFactory,
    CommandParser commandParser,
    MacroSettings settings
) : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    )
    {
        var macroName = request.Args[0].Key;

        if (!settings.Macros.TryGetValue(macroName, out var macro))
        {
            var available = string.Join(", ", settings.Macros.Keys.OrderBy(k => k));
            throw new TagSelectaException($"Unknown macro '{macroName}'. Available: {available}");
        }

        var commands = macro.Split("&&");

        foreach (var command in commands)
        {
            commandParser.TryParse(command, out var parsedRequest);
            await commandFactory
                .Create(parsedRequest.Name)
                .ExecuteAsync(context, parsedRequest, token);
        }
    }
}

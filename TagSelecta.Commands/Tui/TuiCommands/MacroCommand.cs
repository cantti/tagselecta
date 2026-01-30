using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Commands.Tui.TuiCommands;

public class MacroSettings
{
    public Dictionary<string, Macro> Macro { get; set; } = new();
}

public class Macro
{
    public List<string> Aliases { get; set; } = [];
    public List<string> Commands { get; set; } = [];
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
        var macroName = request.Args.FirstOrDefault()?.Key;

        if (string.IsNullOrWhiteSpace(macroName))
        {
            throw new TagSelectaException("Macro name is required.");
        }

        var macro = settings
            .Macro.Where(x => x.Key == macroName || x.Value.Aliases.Contains(macroName))
            .Select(x => x.Value)
            .FirstOrDefault();

        if (macro is null)
        {
            var available = string.Join(", ", settings.Macro.Keys.OrderBy(k => k));
            throw new TagSelectaException($"Unknown macro '{macroName}'. Available: {available}");
        }

        foreach (var command in macro.Commands)
        {
            token.ThrowIfCancellationRequested();

            if (commandParser.TryParse(command, out var parsedRequest))
            {
                await commandFactory
                    .Create(parsedRequest.Name)
                    .ExecuteAsync(context, parsedRequest, token);
            }
            else
            {
                throw new TagSelectaException($"Invalid command: {command}");
            }
        }
    }
}

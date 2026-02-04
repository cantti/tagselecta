using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Commands.Tui.TuiCommands;

public class MacroSettings
{
    public Dictionary<string, string[]> Macros { get; set; } = new();
}

[TuiCommand("macro", "m")]
public class MacroCommand(ITuiCommandFactory commandFactory, MacroSettings settings) : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    )
    {
        var macroName = request.Options.FirstOrDefault()?.Key;

        if (string.IsNullOrWhiteSpace(macroName))
        {
            throw new TagSelectaException("Macro name is required.");
        }

        var macro = settings
            .Macros.Where(x => x.Key == macroName)
            .Select(x => x.Value)
            .FirstOrDefault();

        if (macro is null)
        {
            var available = string.Join(", ", settings.Macros.Keys.OrderBy(k => k));
            throw new TagSelectaException($"Unknown macro '{macroName}'. Available: {available}");
        }

        foreach (var command in macro)
        {
            token.ThrowIfCancellationRequested();

            if (CommandParser.TryParse(command, out var parsedRequest))
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

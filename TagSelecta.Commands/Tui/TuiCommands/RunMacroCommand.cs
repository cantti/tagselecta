using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("run", "r")]
public class RunMacroCommand(MacroConfig config, ITuiCommandDispatcher dispatcher) : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var macroName = parsedCommand.Options.FirstOrDefault()?.Key;

        if (string.IsNullOrWhiteSpace(macroName))
        {
            throw new TagSelectaException("Macro name is required.");
        }

        var macro = config
            .Macros.Where(x => x.Key == macroName)
            .Select(x => x.Value)
            .FirstOrDefault();

        if (macro is null)
        {
            var available = string.Join(", ", config.Macros.Keys.OrderBy(k => k));
            throw new TagSelectaException($"Unknown macro '{macroName}'. Available: {available}");
        }

        var executed = await dispatcher.Execute(macro, context, token);
        if (!executed)
        {
            context.Print($"Invalid macro: {macro}");
        }
    }
}
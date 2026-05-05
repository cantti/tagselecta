using Sprache;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public static class CommandParser
{
    private static readonly Parser<string> _quotedValue =
        from open in Parse.Char('"')
        from value in Parse.CharExcept('"').Many().Text()
        from close in Parse.Char('"')
        select value;

    private static readonly Parser<string> _unquotedValue = Parse
        .CharExcept(" &")
        .AtLeastOnce()
        .Text();

    private static readonly Parser<ParsedCommand> _parsedCommand = (
        from commandName in Parse.Letter.AtLeastOnce().Text()
        from space in Parse.WhiteSpace.Many()
        from commandOption in _commandOption.Or(_commandOptionNoValue).Many()
        select new ParsedCommand(commandName, commandOption.ToArray())
    ).Token();

    private static readonly Parser<IEnumerable<ParsedCommand>> _parsedCommands =
        _parsedCommand.DelimitedBy(Parse.String("&&").Token());

    private static readonly Parser<ParsedCommandOption> _commandOption = (
        from key in Parse.LetterOrDigit.AtLeastOnce().Text()
        from eq in Parse.Char('=')
        from value in _quotedValue.Or(_unquotedValue)
        select new ParsedCommandOption(key, value)
    ).Token();

    private static readonly Parser<ParsedCommandOption> _commandOptionNoValue = (
        from key in Parse.LetterOrDigit.AtLeastOnce().Text()
        select new ParsedCommandOption(key, "")
    ).Token();

    public static bool TryParse(string input, out List<ParsedCommand> parsedCommands)
    {
        parsedCommands = [];

        var result = _parsedCommands.TryParse(input);

        if (!result.WasSuccessful)
        {
            return false;
        }

        parsedCommands = result.Value.ToList();

        return true;
    }
}

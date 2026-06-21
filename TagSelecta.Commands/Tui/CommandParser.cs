using Sprache;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public static class CommandParser
{
    private static readonly Parser<char> _escapedChar =
        from slash in Parse.Char('\\')
        from esc in Parse.Chars('\\', '"', ' ', '=')
        select esc;

    private static readonly Parser<char> _unQuotedChar = _escapedChar.Or(
        Parse.CharExcept(" =\\\"&")
    );

    private static readonly Parser<string> _quoted =
        from open in Parse.Char('"')
        from value in _escapedChar.Or(Parse.CharExcept("\\\"")).Many().Text()
        from close in Parse.Char('"')
        select value;

    private static readonly Parser<ParsedCommand> _parsedCommand = (
        from commandName in Parse.Letter.AtLeastOnce().Text()
        from space in Parse.WhiteSpace.Many()
        from commandOption in _commandOption.Or(_commandOptionFlag).Many()
        select new ParsedCommand(commandName, commandOption.ToArray())
    ).Token();

    private static readonly Parser<IEnumerable<ParsedCommand>> _parsedCommands = _parsedCommand
        .DelimitedBy(Parse.String("&&").Token())
        .End();

    private static readonly Parser<ParsedCommandOption> _commandOption = (
        from key in _quoted.Or(_unQuotedChar.AtLeastOnce().Text())
        from eq in Parse.Char('=')
        from value in _quoted.Or(_unQuotedChar.Many().Text())
        select new ParsedCommandOption(key, value)
    ).Token();

    private static readonly Parser<ParsedCommandOption> _commandOptionFlag = (
        from key in _quoted.Or(_unQuotedChar.AtLeastOnce().Text())
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

using System.Text;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public static class CommandParser
{
    public static bool TryParse(string input, out ParsedCommand[] parsedCommands)
    {
        parsedCommands = null!;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var commandTexts = SplitCommands(input);
        if (commandTexts.Count == 0)
        {
            return false;
        }

        var commands = new List<ParsedCommand>(commandTexts.Count);

        foreach (var commandText in commandTexts)
        {
            var parts = Tokenize(commandText);
            if (parts.Count == 0)
            {
                return false;
            }

            var name = parts[0];
            var options = new List<ParsedCommandOption>();

            for (var i = 1; i < parts.Count; i++)
            {
                var part = parts[i];

                var eq = part.IndexOf('=');
                if (eq > 0)
                {
                    var key = part[..eq];
                    var value = part[(eq + 1)..];
                    options.Add(new ParsedCommandOption(key, value));
                }
                else
                {
                    options.Add(new ParsedCommandOption(part, ""));
                }
            }

            commands.Add(new ParsedCommand(name, options.ToArray()));
        }

        parsedCommands = commands.ToArray();
        return true;
    }

    public static bool TryParseSingle(string input, out ParsedCommand parsedCommand)
    {
        parsedCommand = null!;

        if (!TryParse(input, out var parsedCommands))
        {
            return false;
        }

        if (parsedCommands.Length != 1)
        {
            return false;
        }

        parsedCommand = parsedCommands[0];
        return true;
    }

    private static List<string> SplitCommands(string input)
    {
        // Splits on && that are OUTSIDE quotes.
        // Example: command1 a=b && command2 a=b
        var commands = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (c == '"' && (i == 0 || input[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
                current.Append(c);
                continue;
            }

            if (!inQuotes && c == '&' && i + 1 < input.Length && input[i + 1] == '&')
            {
                var text = current.ToString().Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    commands.Add(text);
                }

                current.Clear();
                i++; // skip second '&'
                continue;
            }

            current.Append(c);
        }

        var last = current.ToString().Trim();
        if (!string.IsNullOrEmpty(last))
        {
            commands.Add(last);
        }

        return commands;
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (c == '"' && (i == 0 || input[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        // unescape \" to "
        for (var i = 0; i < tokens.Count; i++)
        {
            tokens[i] = tokens[i].Replace("\\\"", "\"");
        }

        return tokens;
    }
}

using System.Text;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public static class CommandParser
{
    public static bool TryParse(string input, out Request request)
    {
        request = null!;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var parts = Tokenize(input);
        if (parts.Count == 0)
            return false;

        var name = parts[0];
        var args = new List<TagDataActionArg>();

        for (int i = 1; i < parts.Count; i++)
        {
            var part = parts[i];

            var eq = part.IndexOf('=');
            if (eq > 0)
            {
                var key = part[..eq];
                var value = part[(eq + 1)..];
                args.Add(new TagDataActionArg(key, value));
            }
            else
            {
                args.Add(new TagDataActionArg(part, ""));
            }
        }

        request = new Request(name, args.ToArray());
        return true;
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

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
            tokens.Add(current.ToString());

        // unescape \" to "
        for (int i = 0; i < tokens.Count; i++)
        {
            tokens[i] = tokens[i].Replace("\\\"", "\"");
        }

        return tokens;
    }
}

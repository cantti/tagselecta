using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandPromptWidget(string text, int cursorPos) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable>();
        cols.Add(new Text(":"));

        foreach (var token in Tokenize(text))
        {
            AddTokenAsRenderable(cols, token);
        }

        if (text.Length == cursorPos)
        {
            cols.Add(new Text(" ", GetStyle(TokenKind.Cursor)));
        }

        return ((IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0) }).Render(
            options,
            maxWidth
        );
    }

    private void AddTokenAsRenderable(List<IRenderable> cols, Token token)
    {
        var s = text.Substring(token.Start, token.Length);
        var style = GetStyle(token.Kind);

        var cursorInside = cursorPos >= token.Start && cursorPos < token.Start + token.Length;
        if (!cursorInside)
        {
            cols.Add(new Text(s, style));
            return;
        }

        var beforeLen = cursorPos - token.Start;
        var atLen = 1;
        var afterStart = beforeLen + atLen;

        if (beforeLen > 0)
        {
            cols.Add(new Text(s.Substring(0, beforeLen), style));
        }

        cols.Add(new Text(s.Substring(beforeLen, atLen), GetStyle(TokenKind.Cursor)));

        if (afterStart < s.Length)
        {
            cols.Add(new Text(s.Substring(afterStart), style));
        }
    }

    private static Style GetStyle(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Value => new Style(Color.Yellow),
            TokenKind.QuotedValue => new Style(Color.Yellow),
            TokenKind.Cursor => new Style(decoration: Decoration.Invert),
            _ => Style.Plain,
        };
    }

    private IEnumerable<Token> Tokenize(string input)
    {
        var i = 0;

        while (i < input.Length)
        {
            // value in ""
            if (input[i] == '"')
            {
                var start = i;
                i++; // opening "

                while (i < input.Length)
                {
                    if (input[i] == '\\' && i + 1 < input.Length)
                    {
                        i += 2; // skip escaped char
                        continue;
                    }

                    if (input[i] == '"')
                    {
                        i++; // closing ""
                        break;
                    }

                    i++;
                }

                yield return new Token(start, i - start, TokenKind.QuotedValue);
                continue;
            }

            // highlight value in key=value
            if (input[i] == '=')
            {
                // apply default to =
                yield return new Token(i, 1, TokenKind.Default);
                i++;

                if (i >= input.Length)
                {
                    break;
                }

                if (input[i] == '"')
                {
                    continue;
                }

                // unquoted value until whitespace or &&
                var vStart = i;
                while (i < input.Length)
                {
                    if (char.IsWhiteSpace(input[i]))
                    {
                        break;
                    }

                    if (input[i] == '&' && i + 1 < input.Length && input[i + 1] == '&')
                    {
                        break;
                    }

                    i++;
                }

                if (i > vStart)
                {
                    yield return new Token(vStart, i - vStart, TokenKind.Value);
                }

                continue;
            }

            // everything else default
            var startDefault = i;
            while (i < input.Length && input[i] != '"' && input[i] != '=')
            {
                i++;
            }

            if (i > startDefault)
            {
                yield return new Token(startDefault, i - startDefault, TokenKind.Default);
            }
        }
    }

    private enum TokenKind
    {
        Default,
        Value,
        QuotedValue,
        Cursor,
    }

    private readonly record struct Token(int Start, int Length, TokenKind Kind);
}

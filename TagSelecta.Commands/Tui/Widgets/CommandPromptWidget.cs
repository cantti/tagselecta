using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandPromptWidget(string text, int cursorPos) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable>();
        cols.Add(new Text(":", GetStyle(TokenKind.Command)));

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
            TokenKind.Whitespace => Style.Plain,
            TokenKind.AndAnd => new Style(Color.Grey, decoration: Decoration.Bold),
            TokenKind.Command => new Style(Color.Default, decoration: Decoration.Bold),
            TokenKind.Key => new Style(Color.Blue),
            TokenKind.Equals => new Style(Color.Default),
            TokenKind.Value => new Style(Color.Yellow),
            TokenKind.QuotedValue => new Style(Color.Yellow),
            TokenKind.Cursor => new Style(decoration: Decoration.Invert),
            _ => Style.Plain,
        };
    }

    private static bool IsIdentPart(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '-';
    }

    private IEnumerable<Token> Tokenize(string input)
    {
        var i = 0;

        var expectingCommand = true;
        var expectingKeyOrFlag = false;

        while (i < input.Length)
        {
            var c = input[i];

            if (char.IsWhiteSpace(c))
            {
                var start = i;
                while (i < input.Length && char.IsWhiteSpace(input[i]))
                {
                    i++;
                }

                yield return new Token(start, i - start, TokenKind.Whitespace);
                continue;
            }

            if (c == '&' && i + 1 < input.Length && input[i + 1] == '&')
            {
                yield return new Token(i, 2, TokenKind.AndAnd);
                i += 2;
                expectingCommand = true;
                expectingKeyOrFlag = false;
                continue;
            }

            if (c == '"')
            {
                var start = i;
                i++;

                while (i < input.Length)
                {
                    if (input[i] == '\\' && i + 1 < input.Length)
                    {
                        i += 2;
                        continue;
                    }

                    if (input[i] == '"')
                    {
                        i++;
                        break;
                    }

                    i++;
                }

                yield return new Token(start, i - start, TokenKind.QuotedValue);
                expectingCommand = false;
                expectingKeyOrFlag = true;
                continue;
            }

            if (IsIdentPart(c))
            {
                var start = i;
                i++;
                while (i < input.Length && IsIdentPart(input[i]))
                {
                    i++;
                }

                if (expectingCommand)
                {
                    yield return new Token(start, i - start, TokenKind.Command);
                    expectingCommand = false;
                    expectingKeyOrFlag = true;
                    continue;
                }

                if (i < input.Length && input[i] == '=')
                {
                    yield return new Token(start, i - start, TokenKind.Key);
                    yield return new Token(i, 1, TokenKind.Equals);
                    i++;

                    if (i < input.Length && input[i] != '"' && !char.IsWhiteSpace(input[i]))
                    {
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
                    }

                    expectingKeyOrFlag = true;
                    continue;
                }

                // flag (no =) or key in progress
                if (expectingKeyOrFlag)
                {
                    yield return new Token(start, i - start, TokenKind.Key);
                    continue;
                }

                yield return new Token(start, i - start, TokenKind.Value);
                continue;
            }

            if (c == '=')
            {
                yield return new Token(i, 1, TokenKind.Equals);
                i++;
                continue;
            }

            yield return new Token(i, 1, TokenKind.Value);
            i++;
        }
    }

    private enum TokenKind
    {
        Whitespace,
        AndAnd,
        Command,
        Key,
        Equals,
        Value,
        QuotedValue,
        Cursor,
    }

    private readonly record struct Token(int Start, int Length, TokenKind Kind);
}

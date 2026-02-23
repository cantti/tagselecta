using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandPromptWidget(string text, int cursorPos, string completion) : Renderable
{
    private readonly Style _completionStyle = new(Color.Grey);

    private readonly Style _cursorStyle = new(decoration: Decoration.Invert);

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable>();
        cols.Add(new Text(":"));

        if (text != "")
        {
            foreach (var token in Tokenize(text))
            {
                AddTokenAsRenderable(cols, token);
            }
        }
        else
        {
            cols.Add(new Text(" ", _cursorStyle));
        }

        return ((IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0) }).Render(
            options,
            maxWidth
        );
    }

    private void AddTokenAsRenderable(List<IRenderable> cols, Token token)
    {
        var s = text.Substring(token.Start, token.Length);
        var style = GetTokenStyle(token.Kind);

        var cursorInside = cursorPos >= token.Start && cursorPos < token.Start + token.Length;

        if (!cursorInside)
        {
            cols.Add(new Text(s, style));
            var isLastToken = token.Start + token.Length == text.Length;
            if (isLastToken && cursorPos == text.Length)
            {
                if (!string.IsNullOrEmpty(completion))
                {
                    AddCompletion();
                }
                else
                {
                    cols.Add(new Text(" ", _cursorStyle));
                }
            }
        }
        else
        {
            var beforeLen = cursorPos - token.Start;
            var atLen = 1;
            var afterStart = beforeLen + atLen;

            if (beforeLen > 0)
            {
                cols.Add(new Text(s.Substring(0, beforeLen), style));
            }

            var at = s[beforeLen].ToString();

            if (!string.IsNullOrEmpty(completion) && at == " ")
            {
                AddCompletion();
                cols.Add(new Text(" "));
            }
            else
            {
                cols.Add(new Text(at, _cursorStyle));
            }

            if (afterStart < s.Length)
            {
                cols.Add(new Text(s.Substring(afterStart), style));
            }
        }

        void AddCompletion()
        {
            cols.Add(new Text(completion[0].ToString(), _cursorStyle));
            if (completion.Length > 1)
            {
                cols.Add(new Text(completion[1..], _completionStyle));
            }
        }
    }

    private static Style GetTokenStyle(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Value => new Style(Color.Yellow),
            TokenKind.QuotedValue => new Style(Color.Yellow),
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
    }

    private readonly record struct Token(int Start, int Length, TokenKind Kind);
}

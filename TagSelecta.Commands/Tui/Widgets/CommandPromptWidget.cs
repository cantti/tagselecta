using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandPromptWidget : Renderable
{
    private readonly Style _completionStyle = new(Color.Grey);

    private readonly Style _cursorStyle = new(decoration: Decoration.Invert);
    private readonly string _text;
    private readonly int _cursorPos;
    private readonly string _completion;

    public CommandPromptWidget(string text, int cursorPos, string completion)
    {
        _cursorPos = cursorPos;
        _completion = completion;
        _text = text[0.._cursorPos] + _completion + text[_cursorPos..];
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable>();
        cols.Add(new Text(":"));

        var tokens = Tokenize(_text).ToList();
        foreach (var token in tokens)
        {
            AddTokenAsRenderable(cols, token);
        }

        return ((IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0) }).Render(
            options,
            maxWidth
        );
    }

    private void AddTokenAsRenderable(List<IRenderable> cols, Token token)
    {
        var style = GetTokenStyle(token.Kind);
        cols.Add(new Text(token.Text, style));
    }

    private static Style GetTokenStyle(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Value => new Style(Color.Yellow),
            TokenKind.QuotedValue => new Style(Color.Yellow),
            TokenKind.Completion => new Style(Color.Grey),
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
                if (i == _cursorPos)
                {
                    yield return new Token(_text.Substring(i, 1), TokenKind.Cursor);
                    i++;
                }
                var start = i;
                i++; // opening "

                while (i < input.Length)
                {
                    if (_cursorPos == i)
                    {
                        yield return new Token(
                            _text.Substring(start, i - start),
                            TokenKind.QuotedValue
                        );
                        yield return new Token(_text.Substring(i, 1), TokenKind.Cursor);
                        start = i + 1;
                        i++;
                        continue;
                    }

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

                if (i > start)
                {
                    yield return new Token(
                        _text.Substring(start, i - start),
                        TokenKind.QuotedValue
                    );
                }
                continue;
            }

            // highlight value in key=value
            if (input[i] == '=')
            {
                // apply default to =
                if (i == _cursorPos)
                {
                    yield return new Token(_text.Substring(i, 1), TokenKind.Cursor);
                }
                else
                {
                    yield return new Token(_text.Substring(i, 1), TokenKind.Default);
                }
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

                    if (_cursorPos == i)
                    {
                        if (i > vStart)
                        {
                            yield return new Token(
                                _text.Substring(vStart, i - vStart),
                                TokenKind.Value
                            );
                        }
                        yield return new Token(_text.Substring(i, 1), TokenKind.Cursor);
                        vStart = i + 1;
                    }

                    i++;
                }

                if (i > vStart)
                {
                    yield return new Token(_text.Substring(vStart, i - vStart), TokenKind.Value);
                }
                continue;
            }
            if (i == _cursorPos)
            {
                if (!string.IsNullOrEmpty(_completion))
                {
                    i += _completion.Length;

                    yield return new Token(_text.Substring(_cursorPos, 1), TokenKind.Cursor);

                    if (_completion.Length > 1)
                    {
                        yield return new Token(
                            _text.Substring(_cursorPos + 1, _completion.Length - 1),
                            TokenKind.Completion
                        );
                    }

                    continue;
                }
                else
                {
                    i++;
                    yield return new Token(_text.Substring(_cursorPos, 1), TokenKind.Cursor);
                    continue;
                }
            }

            // everything else default
            var startDefault = i;
            while (i < input.Length && input[i] != '"' && input[i] != '=' && i != _cursorPos)
            {
                i++;
            }

            if (i > startDefault)
            {
                yield return new Token(
                    _text.Substring(startDefault, i - startDefault),
                    TokenKind.Default
                );
            }
        }

        if (input.Length == _cursorPos)
        {
            yield return new Token(" ", TokenKind.Cursor);
        }
    }

    private enum TokenKind
    {
        Default,
        Value,
        QuotedValue,
        Completion,
        Cursor,
    }

    private readonly record struct Token(string Text, TokenKind Kind);
}

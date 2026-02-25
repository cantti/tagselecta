using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandPromptWidget : Renderable
{
    private readonly string _completion;
    private readonly int _cursorPos;
    private readonly string _text;

    public CommandPromptWidget(string text, int cursorPos, string completion)
    {
        _cursorPos = cursorPos;
        _completion = completion;
        _text = text[.._cursorPos] + _completion + text[_cursorPos..];
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable> { new Text(":") };
        var tokens = Tokenize().ToList();
        foreach (var token in tokens)
        {
            var style = GetTokenStyle(token.Kind);
            cols.Add(new Text(token.Char.ToString(), style));
        }

        return ((IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0) }).Render(
            options,
            maxWidth
        );
    }

    private static Style GetTokenStyle(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Value => new Style(Color.Yellow),
            TokenKind.Completion => new Style(Color.Grey),
            TokenKind.Cursor => new Style(decoration: Decoration.Invert),
            _ => Style.Plain,
        };
    }

    private List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        var inQuotes = false;
        var valueStarted = false;

        for (var i = 0; i < _text.Length; i++)
        {
            // current char is quote
            var isQuote = false;

            // toggle inQuotes if we encounter an unescaped "
            if (_text[i] == '"' && (i == 0 || _text[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
                isQuote = true;
            }

            // update valueStarted if previous char is =
            if (!inQuotes && i > 0 && _text[i - 1] == '=')
            {
                valueStarted = true;
            }

            // reset valueStarted if we encounter a space
            if (_text[i] == ' ')
            {
                valueStarted = false;
            }

            // check if we are in the completion range
            var inCompletion =
                !string.IsNullOrEmpty(_completion)
                && i >= _cursorPos
                && i < _cursorPos + _completion.Length;

            // finally render
            if (i == _cursorPos)
            {
                tokens.Add(new Token(_text[i], TokenKind.Cursor));
            }
            else if (inCompletion)
            {
                tokens.Add(new Token(_text[i], TokenKind.Completion));
            }
            else if (inQuotes || valueStarted || isQuote)
            {
                tokens.Add(new Token(_text[i], TokenKind.Value));
            }
            else
            {
                tokens.Add(new Token(_text[i], TokenKind.Default));
            }
        }

        if (_text.Length == _cursorPos)
        {
            tokens.Add(new Token(' ', TokenKind.Cursor));
        }

        return tokens;
    }

    private enum TokenKind
    {
        Default,
        Value,
        Completion,
        Cursor,
    }

    private readonly record struct Token(char Char, TokenKind Kind);
}

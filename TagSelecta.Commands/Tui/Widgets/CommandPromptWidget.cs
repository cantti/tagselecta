using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandPromptWidget : Renderable
{
    private readonly string _completion;
    private readonly int _cursorPos;
    private readonly TuiAppTheme _theme;
    private readonly string _text;

    public CommandPromptWidget(TuiAppTheme theme, string text, int cursorPos, string completion)
    {
        _theme = theme;
        _text = text;
        _cursorPos = cursorPos;
        _completion = completion;
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var fullText = ":" + _text[.._cursorPos] + _completion + _text[_cursorPos..] + " ";
        var cursorPos = _cursorPos + 1; // compensate :

        var completionEnd = cursorPos + _completion.Length;
        if (completionEnd >= maxWidth)
        {
            var windowStart = completionEnd - maxWidth + 1;
            fullText = fullText[windowStart..(windowStart + maxWidth)];
            cursorPos -= windowStart;
        }
        else
        {
            fullText = fullText[..Math.Min(maxWidth, fullText.Length)];
        }

        fullText = fullText.PadRight(maxWidth);

        var segments = new List<Segment>();

        AddTextSegment(segments, fullText[..cursorPos], TokenKind.Default);
        AddTextSegment(segments, fullText[cursorPos].ToString(), TokenKind.Cursor);

        if (_completion.Length > 0)
        {
            var afterCompletionStart = cursorPos + _completion.Length;
            AddTextSegment(
                segments,
                fullText[(cursorPos + 1)..afterCompletionStart],
                TokenKind.Completion
            );
            AddTextSegment(segments, fullText[afterCompletionStart..], TokenKind.Default);
        }
        else
        {
            AddTextSegment(segments, fullText[(cursorPos + 1)..], TokenKind.Default);
        }

        return segments;
    }

    private void AddTextSegment(List<Segment> segments, string value, TokenKind kind)
    {
        if (value.Length == 0)
        {
            return;
        }

        segments.Add(new Segment(value, GetTokenStyle(kind)));
    }

    private Style GetTokenStyle(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Completion => new Style(
                Color.FromHex(_theme.MutedFg),
                Color.FromHex(_theme.SurfaceBg)
            ),
            TokenKind.Cursor => new Style(decoration: Decoration.Invert),
            _ => new Style(Color.FromHex(_theme.TextFg), Color.FromHex(_theme.SurfaceBg)),
        };
    }

    private enum TokenKind
    {
        Default,
        Completion,
        Cursor,
    }
}

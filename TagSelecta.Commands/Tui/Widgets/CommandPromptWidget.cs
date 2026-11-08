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
        var contentWidth = maxWidth - SectionPanel.HorizontalPadding;
        if (contentWidth <= 0)
        {
            return SectionPanel
                .Create("Command", Text.Empty, _theme.CommandBorder)
                .ToSegments(options, maxWidth);
        }

        var fullText = ":" + _text[.._cursorPos] + _completion + _text[_cursorPos..] + " ";
        var cursorPos = _cursorPos + 1; // compensate :
        var completionEnd = cursorPos + _completion.Length;
        var windowStart = Math.Min(Math.Max(0, completionEnd - contentWidth + 1), cursorPos);
        fullText = fullText[windowStart..Math.Min(fullText.Length, windowStart + contentWidth)]
            .PadRight(contentWidth);
        cursorPos -= windowStart;
        completionEnd = Math.Min(completionEnd - windowStart, contentWidth);

        var normalStyle = new Style(Color.FromHex(_theme.TextFg));
        var completionStyle = new Style(Color.FromHex(_theme.MutedFg));
        var cursorStyle = new Style(decoration: Decoration.Invert);
        var afterCursor = cursorPos + 1;
        var afterCompletion = Math.Max(afterCursor, completionEnd);
        var paragraph = new Paragraph()
            .Append(fullText[..cursorPos], normalStyle)
            .Append(fullText[cursorPos].ToString(), cursorStyle)
            .Append(fullText[afterCursor..afterCompletion], completionStyle)
            .Append(fullText[afterCompletion..], normalStyle);

        return SectionPanel
            .Create("Command", paragraph, _theme.CommandBorder)
            .ToSegments(options, maxWidth);
    }
}

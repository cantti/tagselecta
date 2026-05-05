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
        _text = text;
        _cursorPos = cursorPos;
        _completion = completion;
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cursorPos = Math.Clamp(_cursorPos, 0, _text.Length);
        var fullText = _text[..cursorPos] + _completion + _text[cursorPos..] + " ";
        var cursorPosInFullText = Math.Clamp(cursorPos, 0, fullText.Length - 1);
        var completionEnd = Math.Min(cursorPos + _completion.Length, fullText.Length);

        var cols = new List<IRenderable> { new Text(":") };

        AddTextSegment(cols, fullText[..cursorPosInFullText], TokenKind.Default);
        AddTextSegment(cols, fullText[cursorPosInFullText].ToString(), TokenKind.Cursor);

        if (completionEnd > cursorPosInFullText + 1)
        {
            AddTextSegment(
                cols,
                fullText[(cursorPosInFullText + 1)..completionEnd],
                TokenKind.Completion
            );
        }

        AddTextSegment(cols, fullText[completionEnd..], TokenKind.Default);

        return ((IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0) }).Render(
            options,
            maxWidth
        );
    }

    private static void AddTextSegment(List<IRenderable> cols, string value, TokenKind kind)
    {
        if (value.Length == 0)
        {
            return;
        }

        cols.Add(new Text(value, GetTokenStyle(kind)));
    }

    private static Style GetTokenStyle(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Completion => new Style(Color.Grey),
            TokenKind.Cursor => new Style(decoration: Decoration.Invert),
            _ => Style.Plain,
        };
    }

    private enum TokenKind
    {
        Default,
        Completion,
        Cursor,
    }
}

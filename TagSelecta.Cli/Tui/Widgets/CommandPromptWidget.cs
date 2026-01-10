using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui.Widgets;

public class CommandPromptWidget(string text, int cursorPos) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable>();
        cols.Add(new Text(":"));
        for (var i = 0; i < text.Length; i++)
        {
            cols.Add(
                new Text(
                    text[i].ToString(),
                    new Style(decoration: i == cursorPos ? Decoration.Invert : Decoration.None)
                )
            );
        }
        if (text.Length == cursorPos)
        {
            cols.Add(new Text(" ", new Style(decoration: Decoration.Invert)));
        }
        return (
            (IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0, 0, 0, 0) }
        ).Render(options, maxWidth);
    }
}

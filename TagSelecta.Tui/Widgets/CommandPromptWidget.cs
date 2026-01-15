using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Tui.Widgets;

public class CommandPromptWidget(string text, int cursorPos) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var cols = new List<IRenderable>();
        cols.Add(new Text(":"));
        for (var i = 0; i < text.Length; i++)
        {
            var characterString = text[i].ToString();
            Style style = new Style();
            if (i == cursorPos)
            {
                style = new Style(decoration: i == cursorPos ? Decoration.Invert : Decoration.None);
            }
            else if (characterString == "=")
            {
                style = new Style(Color.Blue);
            }
            cols.Add(new Text(characterString, style));
        }
        if (text.Length == cursorPos)
        {
            cols.Add(new Text(" ", new Style(decoration: Decoration.Invert)));
        }
        return ((IRenderable)new Columns(cols) { Expand = false, Padding = new Padding(0) }).Render(
            options,
            maxWidth
        );
    }
}

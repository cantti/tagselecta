using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class HeaderWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (maxWidth <= 0)
        {
            return [];
        }

        var keys = new List<(string Key, string Action)>
        {
            ("q", "quit"),
            ("j/k", "move"),
            ("space", "select"),
            ("e", "file list"),
            ("p", "picture"),
            ("h", "command help"),
            ("?", "hotkeys"),
        };

        var keyLine = string.Join(", ", keys.Select(x => $"[{x.Key}] {x.Action}"));

        const string title = "Tagselecta";
        var text = $"{title} {keyLine}";
        return ((IRenderable)new SectionHeaderWidget(text)).Render(options, maxWidth);

        var titleVisible = title[..Math.Min(title.Length, maxWidth)];
        var remaining = maxWidth - titleVisible.Length;

        var tailVisible = "";
        if (remaining > 0)
        {
            var tail = "  " + keyLine;
            tailVisible = tail.Length > remaining ? tail[..remaining] : tail.PadRight(remaining);
        }

        return [new Segment(titleVisible, new Style(Color.Black, Color.Green))];
    }
}

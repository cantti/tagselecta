using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public static class RenderableExtensions
{
    public static IEnumerable<Segment> ToSegments(
        this IRenderable renderable,
        RenderOptions options,
        int maxWidth
    )
    {
        return renderable.Render(options, maxWidth);
    }
}

using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class TagDataWidget(TagDataActionTarget? focusedFile) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var rows = new List<IRenderable>();
        rows.Add(new SectionHeaderWidget("Metadata:"));

        if (focusedFile?.Exception is not null)
        {
            rows.Add(new Text($"Error: {focusedFile.Exception.Message}", new Style(Color.Red)));
        }

        var tagDataRenderable = focusedFile is not null
            ? TagDataPrinter.PrintComparison(focusedFile)
            : Text.Empty;

        rows.Add(tagDataRenderable);

        return ((IRenderable)new Rows(rows)).Render(options, maxWidth);
    }
}

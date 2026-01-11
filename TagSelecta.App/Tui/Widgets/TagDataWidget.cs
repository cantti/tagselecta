using Spectre.Console;
using Spectre.Console.Rendering;
using TagSelecta.Tagging;

namespace TagSelecta.App.Tui.Widgets;

public class TagDataWidget(TagDataOperation? focusedOperation) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var rows = new List<IRenderable>();
        rows.Add(new Text("Metadata:", new Style(Color.Yellow, Color.Default, Decoration.Bold)));

        var tagDataRenderable = focusedOperation is not null
            ? focusedOperation.HasChanges
                ? TagDataPrinter.PrintComparison(focusedOperation)
                : TagDataPrinter.PrintTagData(focusedOperation)
            : TagDataPrinter.PrintTagData(new TagDataOperation("", new TagData()));

        rows.Add(tagDataRenderable);

        if (focusedOperation?.Exception is not null)
        {
            rows.Add(Text.NewLine);
            rows.Add(
                new Text($"Error: {focusedOperation.Exception.Message}", new Style(Color.Red))
            );
        }

        return ((IRenderable)new Rows(rows)).Render(options, maxWidth);
    }
}

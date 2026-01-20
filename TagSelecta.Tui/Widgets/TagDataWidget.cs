using Spectre.Console;
using Spectre.Console.Rendering;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Tui.Widgets;

public class TagDataWidget(TagDataActionTarget? focusedFile) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var rows = new List<IRenderable>();
        rows.Add(new Text("Metadata:", new Style(Color.Yellow, Color.Default, Decoration.Bold)));

        var tagDataRenderable = focusedFile is not null
            ? focusedFile.HasChanges
                ? TagDataPrinter.PrintComparison(focusedFile)
                : TagDataPrinter.PrintTagData(focusedFile)
            : Text.Empty;

        rows.Add(tagDataRenderable);

        if (focusedFile?.Exception is not null)
        {
            rows.Add(Text.NewLine);
            rows.Add(new Text($"Error: {focusedFile.Exception.Message}", new Style(Color.Red)));
        }

        return ((IRenderable)new Rows(rows)).Render(options, maxWidth);
    }
}

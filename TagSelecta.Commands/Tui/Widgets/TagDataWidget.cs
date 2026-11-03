using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class TagDataWidget(TuiAppTheme theme, TagDataActionTarget? focusedFile) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var rows = new List<IRenderable>();
        rows.Add(new SectionHeaderWidget(theme, "Metadata:"));

        if (focusedFile?.Exception is not null)
        {
            rows.Add(
                new Text(
                    $"Error: {focusedFile.Exception.Message}",
                    new Style(Color.FromHex(theme.DangerFg))
                )
            );
        }

        var tagDataRenderable = focusedFile is not null
            ? TagDataPrinter.PrintComparison(theme, focusedFile)
            : Text.Empty;

        rows.Add(tagDataRenderable);

        return new Rows(rows).ToSegments(options, maxWidth);
    }
}

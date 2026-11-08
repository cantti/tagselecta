using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class TagDataWidget(TuiAppTheme theme, TagDataActionTarget? focusedFile, int offset)
    : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var rows = new List<IRenderable>();

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
            ? TagDataPrinter.CreateComparisonTable(theme, focusedFile, offset)
            : Text.Empty;

        rows.Add(tagDataRenderable);

        return SectionPanel
            .Create("Metadata", new Rows(rows), theme.Border)
            .ToSegments(options, maxWidth);
    }
}

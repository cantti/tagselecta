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

        var fieldCount = focusedFile is not null ? TagDataRenderer.GetFieldCount(focusedFile) : 0;
        var visibleOffset = Math.Clamp(offset, 0, Math.Max(0, fieldCount - 1));
        var tagDataRenderable = focusedFile is not null
            ? TagDataRenderer.CreateComparisonTable(theme, focusedFile, visibleOffset)
            : Text.Empty;

        rows.Add(tagDataRenderable);

        var header = $"Metadata ({fieldCount})";
        return SectionPanel
            .Create(header, new Rows(rows), theme.Border)
            .ToSegments(options, maxWidth);
    }
}

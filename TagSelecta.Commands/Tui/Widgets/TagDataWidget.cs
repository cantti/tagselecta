using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class TagDataWidget(
    TuiAppTheme theme,
    Dictionary<string, string> fieldIcons,
    TagDataActionTarget? focusedFile,
    int offset
) : Renderable
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

        var fields = focusedFile is not null ? TagDataComparison.GetFields(focusedFile) : [];
        var visibleOffset = Math.Clamp(offset, 0, Math.Max(0, fields.Count - 1));
        var tagDataRenderable = focusedFile is not null
            ? CreateComparisonTable(fields, visibleOffset)
            : Text.Empty;

        rows.Add(tagDataRenderable);

        var header = $"Metadata ({fields.Count})";
        return SectionPanel
            .Create(header, new Rows(rows), theme.Border)
            .ToSegments(options, maxWidth);
    }

    private IRenderable CreateComparisonTable(
        List<TagDataComparison.Field> fields,
        int visibleOffset
    )
    {
        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(null));
        table.AddColumn("");
        table.HideHeaders();

        var pictureHeaderAdded = false;
        foreach (var field in fields.Skip(visibleOffset))
        {
            if (field.IsPicture && !pictureHeaderAdded)
            {
                if (table.Rows.Count > 0)
                {
                    table.AddEmptyRow();
                }

                table.AddRow(
                    new Text(
                        "Pictures:",
                        new Style(Color.FromHex(theme.AccentFg), decoration: Decoration.Italic)
                    )
                );
                pictureHeaderAdded = true;
            }

            AddComparisonRow(table, field);
        }

        return table;
    }

    private void AddComparisonRow(Table table, TagDataComparison.Field field)
    {
        var label = theme.UseNerdFontIcons
            ? $"{fieldIcons.GetValueOrDefault(field.IsPicture ? "picture" : field.Key, " ")}  {field.Key}"
            : field.Key;
        var keyCol = new Text(label, new Style(Color.FromHex(theme.AccentFg)));
        var oldValue = field.IsPicture ? field.OldValue.EscapeMarkup() : field.OldValue;
        var newValue = field.IsPicture ? field.NewValue.EscapeMarkup() : field.NewValue;

        var elements = new List<IRenderable>();
        if (field.Equal)
        {
            elements.Add(new Text(oldValue, new Style(Color.FromHex(theme.TextFg))));
        }
        else
        {
            if (!string.IsNullOrEmpty(oldValue))
            {
                elements.Add(new Text(oldValue, new Style(Color.FromHex(theme.DangerFg))));
            }

            if (!string.IsNullOrEmpty(newValue) && !string.IsNullOrEmpty(oldValue))
            {
                elements.Add(new Text(theme.ChangeSymbol, new Style(Color.FromHex(theme.MutedFg))));
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                elements.Add(new Text(newValue, new Style(Color.FromHex(theme.SuccessFg))));
            }
        }

        var cols = new Columns(elements) { Expand = false };
        table.AddRow(keyCol, cols);
    }
}

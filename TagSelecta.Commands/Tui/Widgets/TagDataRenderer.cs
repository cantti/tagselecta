using Spectre.Console;
using Spectre.Console.Rendering;
using TagLib;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tui.Widgets;

public static class TagDataRenderer
{
    public static IRenderable CreateComparisonTable(
        TuiAppTheme theme,
        Dictionary<string, string> fieldIcons,
        TagDataActionTarget f,
        int offset
    )
    {
        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(null));
        table.AddColumn("");
        table.HideHeaders();

        var fields = GetFields(f);
        offset = Math.Clamp(offset, 0, Math.Max(0, fields.Count - 1));
        var pictureHeaderAdded = false;
        foreach (var field in fields.Skip(offset))
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

            AddComparisonRow(theme, fieldIcons, table, field);
        }

        return table;
    }

    public static int GetFieldCount(TagDataActionTarget f)
    {
        return GetFields(f).Count;
    }

    private static List<ComparisonField> GetFields(TagDataActionTarget f)
    {
        var tagData = f.CurrentTagData;
        var backupTagData = f.BackupTagData;
        var fields = new List<ComparisonField>();

        AddField("Path", f.BackupPath, f.CurrentPath);

        foreach (
            var key in backupTagData
                .Fields.Select(x => x.Key)
                .Union(tagData.Fields.Select(x => x.Key))
                .Order()
        )
        {
            var value1 = ListToStr(
                backupTagData.Fields.SingleOrDefault(x => x.Key == key)?.Text ?? []
            );
            var value2 = ListToStr(tagData.Fields.SingleOrDefault(x => x.Key == key)?.Text ?? []);
            AddField(key, value1, value2);
        }

        if (backupTagData.Picture.Count > 0 || tagData.Picture.Count > 0)
        {
            // collect unique picture types
            var types = backupTagData
                .Picture.Select(p => p.Type)
                .Union(tagData.Picture.Select(p => p.Type))
                .OrderBy(t => t.ToString())
                .ToList();

            foreach (var type in types)
            {
                var list1 = backupTagData.Picture.Where(p => p.Type == type).ToList();
                var list2 = tagData.Picture.Where(p => p.Type == type).ToList();

                var max = Math.Max(list1.Count, list2.Count);

                for (var i = 0; i < max; i++)
                {
                    var p1 = i < list1.Count ? list1[i] : null;
                    var p2 = i < list2.Count ? list2[i] : null;
                    var value1 = PictureToStr(p1);
                    var value2 = PictureToStr(p2);
                    AddField(
                        type.ToString(),
                        value1,
                        value2,
                        TagDataComparer.PictureEq(p1, p2),
                        isPicture: true
                    );
                }
            }
        }

        return fields;

        void AddField(
            string key,
            string oldValue,
            string newValue,
            bool? equal = null,
            bool isPicture = false
        )
        {
            if (!string.IsNullOrEmpty(oldValue) || !string.IsNullOrEmpty(newValue))
            {
                fields.Add(new ComparisonField(key, oldValue, newValue, equal, isPicture));
            }
        }
    }

    private static void AddComparisonRow(
        TuiAppTheme theme,
        Dictionary<string, string> fieldIcons,
        Table table,
        ComparisonField field
    )
    {
        if (string.IsNullOrEmpty(field.OldValue) && string.IsNullOrEmpty(field.NewValue))
        {
            return;
        }

        var label = theme.UseNerdFontIcons
            ? $"{fieldIcons.GetValueOrDefault(field.IsPicture ? "picture" : field.Key, " ")}  {field.Key}"
            : field.Key;
        var keyCol = new Text(label, new Style(Color.FromHex(theme.AccentFg)));

        var elements = new List<IRenderable>();
        if (field.Equal ?? field.OldValue == field.NewValue)
        {
            elements.Add(new Text(field.OldValue, new Style(Color.FromHex(theme.TextFg))));
        }
        else
        {
            if (!string.IsNullOrEmpty(field.OldValue))
            {
                elements.Add(new Text(field.OldValue, new Style(Color.FromHex(theme.DangerFg))));
            }

            if (!string.IsNullOrEmpty(field.NewValue) && !string.IsNullOrEmpty(field.OldValue))
            {
                elements.Add(new Text(theme.ChangeSymbol, new Style(Color.FromHex(theme.MutedFg))));
            }

            if (!string.IsNullOrEmpty(field.NewValue))
            {
                elements.Add(new Text(field.NewValue, new Style(Color.FromHex(theme.SuccessFg))));
            }
        }

        var cols = new Columns(elements) { Expand = false };
        table.AddRow(keyCol, cols);
    }

    private static string PictureToStr(Picture? pic)
    {
        return pic is not null ? $"{pic.Data.Count / 1024} KB, {pic.MimeType}".EscapeMarkup() : "";
    }

    private static string ListToStr(List<string> list)
    {
        return string.Join("\n", list);
    }

    private record ComparisonField(
        string Key,
        string OldValue,
        string NewValue,
        bool? Equal,
        bool IsPicture
    );
}

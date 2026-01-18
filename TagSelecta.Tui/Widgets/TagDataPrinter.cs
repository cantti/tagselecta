using Spectre.Console;
using Spectre.Console.Rendering;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Tui.Widgets;

public static class TagDataPrinter
{
    public static IRenderable PrintTagData(TagDataOperation f)
    {
        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(20));
        table.AddColumn("");
        table.HideHeaders();
        AddField(table, "Path", f.CurrentPath);
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
        )
        {
            AddField(
                table,
                prop.Name,
                ConvertValue(prop.GetValue(f.CurrentTagData), prop.PropertyType)
            );
        }
        if (f.CurrentTagData.Custom.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Custom:[/]");
            foreach (var custom in f.CurrentTagData.Custom)
            {
                AddField(table, custom.Key, custom.Text);
            }
        }
        if (f.CurrentTagData.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");
            foreach (var picture in f.CurrentTagData.Picture)
            {
                AddField(table, picture.Type.ToString(), PictureToStr(picture));
            }
        }
        return table;
    }

    public static IRenderable PrintComparison(TagDataOperation f)
    {
        // cache backup value
        var backupTagData = f.BackupTagData;

        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(20));
        table.AddColumn("");
        table.HideHeaders();

        AddFieldComparison(table, "Path", f.BackupPath, f.CurrentPath);

        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
        )
        {
            AddFieldComparison(
                table,
                prop.Name,
                ConvertValue(prop.GetValue(backupTagData), prop.PropertyType),
                ConvertValue(prop.GetValue(f.CurrentTagData), prop.PropertyType)
            );
        }
        var customKeys = backupTagData
            .Custom.Select(x => x.Key)
            .Union(f.CurrentTagData.Custom.Select(x => x.Key))
            .ToList();
        if (customKeys.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Custom:[/]");
            foreach (
                var key in backupTagData
                    .Custom.Select(x => x.Key)
                    .Union(f.CurrentTagData.Custom.Select(x => x.Key))
            )
            {
                var value1 = backupTagData.Custom.SingleOrDefault(x => x.Key == key)?.Text ?? "";
                var value2 = f.CurrentTagData.Custom.SingleOrDefault(x => x.Key == key)?.Text ?? "";
                AddFieldComparison(table, key, value1, value2);
            }
        }

        if (backupTagData.Picture.Count > 0 || f.CurrentTagData.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");

            // collect unique picture types
            var types = backupTagData
                .Picture.Select(p => p.Type)
                .Union(f.CurrentTagData.Picture.Select(p => p.Type))
                .OrderBy(t => t.ToString())
                .ToList();

            foreach (var type in types)
            {
                var list1 = backupTagData.Picture.Where(p => p.Type == type).ToList();
                var list2 = f.CurrentTagData.Picture.Where(p => p.Type == type).ToList();

                int max = Math.Max(list1.Count, list2.Count);

                for (int i = 0; i < max; i++)
                {
                    var p1 = i < list1.Count ? list1[i] : null;
                    var p2 = i < list2.Count ? list2[i] : null;
                    string value1 = PictureToStr(p1);
                    string value2 = PictureToStr(p2);
                    AddFieldComparison(
                        table,
                        type.ToString(),
                        value1,
                        value2,
                        TagDataComparer.PictureEq(p1, p2)
                    );
                }
            }
        }
        return table;
    }

    private static void AddField(Table table, string label, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        table.AddRow([$"[blue]{label.ToSpacedWords().EscapeMarkup()}[/]", value.EscapeMarkup()]);
    }

    private static void AddFieldComparison(
        Table table,
        string label,
        string value1,
        string value2,
        bool? eq = null
    )
    {
        if (string.IsNullOrEmpty(value1) && string.IsNullOrEmpty(value2))
        {
            return;
        }
        var labelText = new Text(label.ToSpacedWords(), new Style(Color.Blue));
        var elements = new List<IRenderable>();
        if (eq ?? value1 == value2)
        {
            elements.Add(new Text(value1, new Style(Color.Default)));
        }
        else
        {
            if (!string.IsNullOrEmpty(value1))
            {
                elements.Add(new Text(value1, new Style(Color.Red)));
            }
            if (!string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value1))
            {
                elements.Add(new Text("➔"));
            }
            if (!string.IsNullOrEmpty(value2))
            {
                elements.Add(new Text(value2, new Style(Color.Green)));
            }
        }

        var cols = new Columns(elements) { Expand = false };
        table.AddRow(labelText, cols);
    }

    private static string PictureToStr(TagLib.Picture? pic)
    {
        return pic is not null ? $"{pic.Data.Count / 1024} KB, {pic.MimeType}".EscapeMarkup() : "";
    }

    private static string ListToStr(List<string> list)
    {
        return string.Join("\n", list);
    }

    static string ConvertValue(object? value, Type type)
    {
        if (value is null)
            return string.Empty;

        if (type == typeof(string))
            return (string)value;

        if (type == typeof(List<string>))
            return ListToStr((List<string>)value);

        throw new TagSelectaException($"Unsupported property type: {type}");
    }
}

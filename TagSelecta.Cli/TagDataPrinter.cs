using Spectre.Console;
using Spectre.Console.Rendering;
using TagSelecta.Shared;
using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public static class TagDataPrinter
{
    public static IRenderable PrintTagData(IAnsiConsole console, TagData tagData)
    {
        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(15));
        table.AddColumn("");
        table.HideHeaders();
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
        )
        {
            AddField(table, prop.Name, ConvertValue(prop.GetValue(tagData), prop.PropertyType));
        }
        if (tagData.Custom.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Custom:[/]");
            foreach (var custom in tagData.Custom)
            {
                AddField(table, custom.Key, custom.Text);
            }
        }
        if (tagData.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");
            foreach (var picture in tagData.Picture)
            {
                AddField(table, picture.Type.ToString(), PictureToStr(picture));
            }
        }
        return new Rows(
            new Text("Metadata:", new Style(Color.Yellow, Color.Default, Decoration.Bold)),
            table
        );
    }

    public static IRenderable PrintComparison(IAnsiConsole console, TagData t1, TagData t2)
    {
        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(15));
        table.AddColumn("");
        table.HideHeaders();

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
                ConvertValue(prop.GetValue(t1), prop.PropertyType),
                ConvertValue(prop.GetValue(t2), prop.PropertyType)
            );
        }
        var customKeys = t1.Custom.Select(x => x.Key).Union(t2.Custom.Select(x => x.Key)).ToList();
        if (customKeys.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Custom:[/]");
            foreach (var key in t1.Custom.Select(x => x.Key).Union(t2.Custom.Select(x => x.Key)))
            {
                var value1 = t1.Custom.SingleOrDefault(x => x.Key == key)?.Text ?? "";
                var value2 = t2.Custom.SingleOrDefault(x => x.Key == key)?.Text ?? "";
                AddFieldComparison(table, key, value1, value2);
            }
        }

        if (t1.Picture.Count > 0 || t2.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");

            // collect unique picture types
            var types = t1
                .Picture.Select(p => p.Type)
                .Union(t2.Picture.Select(p => p.Type))
                .OrderBy(t => t.ToString())
                .ToList();

            foreach (var type in types)
            {
                var list1 = t1.Picture.Where(p => p.Type == type).ToList();
                var list2 = t2.Picture.Where(p => p.Type == type).ToList();

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
        return new Rows(new Text("Metadata:", new Style(Color.Yellow)), table);
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
        var areEqual = eq ?? value1 == value2;
        var color1 = areEqual ? "[white]" : "[red]";
        var color2 = areEqual ? "[white]" : "[green]";
        var rowText = areEqual
            ? $"{color1}{value1.EscapeMarkup()}[/]"
            : $"{color1}{value1.EscapeMarkup()}[/] ➔ {color2}{value2.EscapeMarkup()}[/]";
        table.AddRow([$"[blue]{label.ToSpacedWords().EscapeMarkup()}[/]", rowText]);
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

        throw new InvalidOperationException($"Unsupported property type: {type}");
    }
}

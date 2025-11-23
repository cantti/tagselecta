using System.Reflection;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public static class TagDataPrinter
{
    public static void PrintTagData(IAnsiConsole console, TagData tagData)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn("");
        table.AddColumn("");
        table.HideHeaders();
        foreach (var prop in typeof(TagData).GetProperties())
        {
            var attr = prop.GetCustomAttribute<TagDataFieldAttribute>();
            if (attr is null)
                continue;
            var value = prop.GetValue(tagData);
            var column = TagDataFieldToColumn(value);
            if (column == "")
                continue;
            table.AddRow([$"[blue]{attr.Label.EscapeMarkup()}[/]", column.EscapeMarkup()]);
        }
        if (tagData.Custom.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Custom tags:[/]");
            foreach (var custom in tagData.Custom)
            {
                table.AddRow([$"[blue]{custom.Key.EscapeMarkup()}[/]", custom.Text.EscapeMarkup()]);
            }
        }
        if (tagData.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");
            foreach (var picture in tagData.Picture)
            {
                table.AddRow(
                    [$"[blue]{picture.Type.ToString().EscapeMarkup()}[/]", PictureToColumn(picture)]
                );
            }
        }
        console.Write(table);
    }

    public static void PrintComparison(IAnsiConsole console, TagData tagData1, TagData tagData2)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn("[yellow]Field[/]");
        table.AddColumn("[yellow]Old Value[/]");
        table.AddColumn("[yellow]New Value[/]");
        foreach (var prop in typeof(TagData).GetProperties())
        {
            var attr = prop.GetCustomAttribute<TagDataFieldAttribute>();
            if (attr is null)
                continue;
            var value1 = prop.GetValue(tagData1)!;
            var value2 = prop.GetValue(tagData2)!;
            var column1 = TagDataFieldToColumn(value1);
            var column2 = TagDataFieldToColumn(value2);
            var areEqual =
                value1 is List<string> l1 && value2 is List<string> l2
                    ? l1.SequenceEqual(l2)
                    : value1.Equals(value2);
            var color1 = areEqual ? "[white]" : "[red]";
            var color2 = areEqual ? "[white]" : "[green]";
            if (column1 == "" && column2 == "")
                continue;
            table.AddRow(
                [
                    $"[blue]{attr.Label.EscapeMarkup()}[/]",
                    $"{color1}{column1.EscapeMarkup()}[/]",
                    $"{color2}{column2.EscapeMarkup()}[/]",
                ]
            );
        }
        var customKeys = tagData1
            .Custom.Select(x => x.Key)
            .Union(tagData2.Custom.Select(x => x.Key))
            .ToList();
        if (customKeys.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Custom tags:[/]");
            foreach (
                var key in tagData1
                    .Custom.Select(x => x.Key)
                    .Union(tagData2.Custom.Select(x => x.Key))
            )
            {
                var value1 = tagData1.Custom.SingleOrDefault(x => x.Key == key)?.Text;
                var value2 = tagData2.Custom.SingleOrDefault(x => x.Key == key)?.Text;
                var areEqual = value1 == value2;
                var color1 = areEqual ? "[white]" : "[red]";
                var color2 = areEqual ? "[white]" : "[green]";
                table.AddRow(
                    [
                        $"[blue]{key.EscapeMarkup()}[/]",
                        $"{color1}{value1.EscapeMarkup()}[/]",
                        $"{color2}{value2.EscapeMarkup()}[/]",
                    ]
                );
            }
        }

        if (tagData1.Picture.Count > 0 || tagData2.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");

            // collect unique picture types
            var types = tagData1
                .Picture.Select(p => p.Type)
                .Union(tagData2.Picture.Select(p => p.Type))
                .OrderBy(t => t.ToString())
                .ToList();

            foreach (var type in types)
            {
                var list1 = tagData1.Picture.Where(p => p.Type == type).ToList();
                var list2 = tagData2.Picture.Where(p => p.Type == type).ToList();

                int max = Math.Max(list1.Count, list2.Count);

                for (int i = 0; i < max; i++)
                {
                    var p1 = i < list1.Count ? list1[i] : null;
                    var p2 = i < list2.Count ? list2[i] : null;

                    bool equal = TagDataComparer.PicturesEqual(p1, p2);

                    string color1 = equal ? "[white]" : "[red]";
                    string color2 = equal ? "[white]" : "[green]";

                    string value1 = PictureToColumn(p1);
                    string value2 = PictureToColumn(p2);

                    table.AddRow(
                        $"[blue]{type.ToString().EscapeMarkup()}[/]",
                        $"{color1}{value1}[/]",
                        $"{color2}{value2}[/]"
                    );
                }
            }
        }
        console.Write(table);
    }

    private static string PictureToColumn(TagLib.Picture? pic)
    {
        return pic is not null ? $"{pic.Data.Count / 1024} KB, {pic.MimeType}".EscapeMarkup() : "";
    }

    private static string TagDataFieldToColumn(object? value)
    {
        if (value is List<string> list)
        {
            return string.Join("\n", list);
        }
        else
        {
            // that will work fine for both uint and double?
            var column = value?.ToString() ?? "";
            column = (value is int int1 && int1 == 0) ? "" : column;
            return column;
        }
    }
}

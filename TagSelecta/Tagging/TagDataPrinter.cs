using System.Reflection;
using Spectre.Console;

namespace TagSelecta.Tagging;

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
            var attr = prop.GetCustomAttribute<EditableAttribute>();
            if (attr is null)
                continue;
            var label = attr.Label ?? prop.Name;
            var value = prop.GetValue(tagData);
            var column = ValueToColumn(value);
            if (column == "")
                continue;
            table.AddRow([$"[blue]{label.EscapeMarkup()}[/]", column.EscapeMarkup()]);
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
            var attr = prop.GetCustomAttribute<EditableAttribute>();
            if (attr is null)
                continue;
            var label = attr.Label ?? prop.Name;
            var value1 = prop.GetValue(tagData1);
            var value2 = prop.GetValue(tagData2);
            var column1 = ValueToColumn(value1);
            var column2 = ValueToColumn(value2);
            var areEqual = TagDataComparer.ValueEquals(value1, value2);
            var color1 = areEqual ? "[white]" : "[red]";
            var color2 = areEqual ? "[white]" : "[green]";
            if (column1 == "" && column2 == "")
                continue;
            table.AddRow(
                [
                    $"[blue]{label.EscapeMarkup()}[/]",
                    $"{color1}{column1.EscapeMarkup()}[/]",
                    $"{color2}{column2.EscapeMarkup()}[/]",
                ]
            );
        }
        console.Write(table);
    }

    private static string ValueToColumn(object? value)
    {
        if (value is List<string> list)
        {
            return string.Join("\n", list);
        }
        else if (value is List<TagLib.Picture> picture)
        {
            return string.Join(
                "\n\n",
                picture.Select(x =>
                    $"{x.Type}"
                    + (!string.IsNullOrWhiteSpace(x.Description) ? $" ({x.Description})" : "")
                )
            );
        }
        else
        {
            // that will work fine for both uint and double?
            var column = value?.ToString() ?? "";
            column = (value is uint uint1 && uint1 == 0) ? "" : column;
            return column;
        }
    }
}

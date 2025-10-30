using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Json;
using TagSelecta.Tagging;

namespace TagSelecta.Print;

public static class AnsiConsoleExtensions
{
    public static void PrintCurrentFile(
        this IAnsiConsole console,
        string file,
        int index,
        int total
    )
    {
        console.MarkupLineInterpolated(
            $"[dim]>[/] [yellow]({index + 1}/{total})[/] [green]{file}[/]"
        );
    }

    public static void PrintTagData(this IAnsiConsole console, TagData meta)
    {
        var json = new JsonText(JsonSerializer.Serialize(new TagDataForJson(meta)));
        console.Write(json);
        console.WriteLine();
    }
}

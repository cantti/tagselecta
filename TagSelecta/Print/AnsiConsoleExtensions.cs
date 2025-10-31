using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { JsonSerializationModifiers.ApplySkipEmptyOrZero },
        },
    };

    public static void PrintTagData(this IAnsiConsole console, TagData meta)
    {
        var json = new JsonText(JsonSerializer.Serialize(new TagDataForJson(meta), _jsonOpts));
        console.Write(json);
        console.WriteLine();
    }
}

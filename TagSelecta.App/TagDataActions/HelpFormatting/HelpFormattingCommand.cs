using System.ComponentModel;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Tagging;

namespace TagSelecta.App.TagDataActions.HelpFormatting;

public class HelpFormattingCommand(IAnsiConsole console) : Command<HelpFormattingSettings>
{
    protected override int Execute(
        CommandContext context,
        HelpFormattingSettings settings,
        CancellationToken cancellationToken
    )
    {
        console.WriteLine(
            "Tagselecta uses the Scriban template engine when formatting fields and when renaming files or directories."
        );
        console.WriteLine();
        console.WriteLine("Example:");
        console.WriteLine("{{ year }} - {{ album }}");
        console.WriteLine();
        console.WriteLine("Useful links:");
        console.MarkupLine(
            "[link=https://github.com/scriban/scriban/blob/master/doc/language.md]https://github.com/scriban/scriban/blob/master/doc/language.md[/]"
        );
        console.MarkupLine(
            "[link=https://github.com/scriban/scriban/blob/master/doc/builtins.md#string-functions]https://github.com/scriban/scriban/blob/master/doc/builtins.md#string-functions[/]"
        );
        console.WriteLine();
        console.WriteLine("Below is the list of available template fields:");
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Description");
        table.HideHeaders();
        table.Border(TableBorder.None);
        foreach (var prop in typeof(TagDataForTemplate).GetProperties())
        {
            var attr = prop.GetCustomAttribute<DescriptionAttribute>();
            if (attr is null)
            {
                continue;
            }
            table.AddRow($"[blue]{prop.Name.ToLower()}[/]", attr.Description);
        }
        console.Write(table);
        return 0;
    }
}

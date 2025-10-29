using TagSelecta.Tagging;
using Spectre.Console;

namespace TagSelecta.Misc;

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
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold cyan]Field[/]")
            .AddColumn("[bold green]Value[/]");

        void AddRow(string field, object? value)
        {
            string displayValue = value switch
            {
                null => "[grey]—[/]",
                string s when string.IsNullOrWhiteSpace(s) => "[grey]—[/]",
                IEnumerable<string> list => list.Any()
                    ? string.Join(Environment.NewLine, list).EscapeMarkup()
                    : "[grey]—[/]",
                _ => $"[white]{value.ToString().EscapeMarkup()}[/]",
            };
            table.AddRow($"[cyan]{field}[/]", displayValue);
        }

        AddRow("Album Artist", meta.AlbumArtist);
        AddRow("Artist", meta.Artist);
        AddRow("Album", meta.Album);
        AddRow("Title", meta.Title);
        AddRow("Genre", meta.Genre);
        AddRow("Year", meta.Year > 0 ? meta.Year.ToString() : null);
        AddRow("Track", meta.Track > 0 ? meta.Track.ToString() : null);
        AddRow("Track Total", meta.TrackTotal > 0 ? meta.TrackTotal.ToString() : null);
        AddRow("Disc", meta.Disc > 0 ? meta.Disc.ToString() : null);
        AddRow("Disc Total", meta.DiscTotal > 0 ? meta.DiscTotal.ToString() : null);
        AddRow("Label", meta.Label);
        AddRow("Catalog Number", meta.CatalogNumber);
        AddRow("Comments", meta.Comment);
        AddRow(
            "Pictures",
            meta.Pictures.Count > 0 ? meta.Pictures.Select(x => x.Type.ToString()) : null
        );

        console.Write(table);
    }
}

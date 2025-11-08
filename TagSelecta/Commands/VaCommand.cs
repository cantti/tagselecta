using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class VaSettings : FileSettings { }

public class VaCommand(IAnsiConsole console) : FileCommand<VaSettings>(console)
{
    protected override void Execute()
    {
        TagData.Artists = [.. TagData.Artists.Select(NormalizeArtistName)];
        TagData.AlbumArtists = [.. TagData.AlbumArtists.Select(NormalizeArtistName)];
        TagData.Composers = [.. TagData.Composers.Select(NormalizeArtistName)];

        WriteTags();
    }

    private static string NormalizeArtistName(string input)
    {
        var aliases = new[] { "VA", "Various" };

        foreach (var alias in aliases)
        {
            if (string.Equals(input, alias, StringComparison.OrdinalIgnoreCase))
            {
                return "Various Artists";
            }
        }

        return input;
    }
}

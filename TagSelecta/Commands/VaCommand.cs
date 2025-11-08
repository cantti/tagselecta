using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class VaSettings : BaseSettings { }

public class VaCommand(IAnsiConsole console) : TagDataProcessingCommand<VaSettings>(console)
{
    protected override void ProcessTagData()
    {
        TagData.Artists = [.. TagData.Artists.Select(NormalizeArtistName)];
        TagData.AlbumArtists = [.. TagData.AlbumArtists.Select(NormalizeArtistName)];
        TagData.Composers = [.. TagData.Composers.Select(NormalizeArtistName)];
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

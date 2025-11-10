using Spectre.Console;
using TagSelecta.Cli.Commands;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class VaSettings : BaseSettings { }

public class VaAction : TagDataAction<VaSettings>
{
    protected override void ProcessTagData(TagDataActionContext<VaSettings> context)
    {
        context.TagData.Artists = [.. context.TagData.Artists.Select(NormalizeArtistName)];
        context.TagData.AlbumArtists =
        [
            .. context.TagData.AlbumArtists.Select(NormalizeArtistName),
        ];
        context.TagData.Composers = [.. context.TagData.Composers.Select(NormalizeArtistName)];
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

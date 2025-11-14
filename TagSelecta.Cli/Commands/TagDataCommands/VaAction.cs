using Spectre.Console;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class VaSettings : BaseSettings { }

public class VaAction : TagDataAction<VaSettings>
{
    protected override void ProcessTagData(TagDataActionContext<VaSettings> context)
    {
        context.TagData.Artists = context.TagData.Artists.Select(NormalizeArtistName).ToList();
        context.TagData.AlbumArtists = context
            .TagData.AlbumArtists.Select(NormalizeArtistName)
            .ToList();
        context.TagData.Composers = context.TagData.Composers.Select(NormalizeArtistName).ToList();
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

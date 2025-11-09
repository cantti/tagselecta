using Spectre.Console;

namespace TagSelecta.TagDataActions;

public class VaSettings : BaseSettings { }

public class VaAction : ITagDataAction<VaSettings>
{
    public Task ProcessTagData(TagDataActionContext<VaSettings> context)
    {
        context.TagData.Artists = [.. context.TagData.Artists.Select(NormalizeArtistName)];
        context.TagData.AlbumArtists =
        [
            .. context.TagData.AlbumArtists.Select(NormalizeArtistName),
        ];
        context.TagData.Composers = [.. context.TagData.Composers.Select(NormalizeArtistName)];
        return Task.CompletedTask;
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

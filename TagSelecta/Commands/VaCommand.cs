using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class VaSettings : FileSettings { }

public class VaCommand(IAnsiConsole console) : FileCommand<VaSettings>(console)
{
    protected override void Execute(string file, int index)
    {
        var originalTags = Tagger.ReadTags(file);

        var tags = originalTags.Clone();

        tags.Artists = [.. tags.Artists.Select(NormalizeArtistName)];
        tags.AlbumArtists = [.. tags.AlbumArtists.Select(NormalizeArtistName)];
        tags.Composers = [.. tags.Composers.Select(NormalizeArtistName)];

        if (!TagDataChanged(originalTags, tags))
        {
            Skip();
            return;
        }

        if (ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }
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

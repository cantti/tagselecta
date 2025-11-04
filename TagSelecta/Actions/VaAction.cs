using Spectre.Console;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class VaSettings : FileSettings { }

public class VaAction(ActionCommon common, ActionContext<VaSettings> context)
    : IFileAction<VaSettings>
{
    public Task Execute(string file, int index)
    {
        var originalTags = Tagger.ReadTags(file);

        var tags = originalTags.Clone();

        tags.Artist = [.. tags.Artist.Select(NormalizeArtistName)];
        tags.AlbumArtist = [.. tags.AlbumArtist.Select(NormalizeArtistName)];
        tags.Composers = [.. tags.Composers.Select(NormalizeArtistName)];

        if (!common.TagDataChanged(originalTags, tags))
        {
            context.Skip();
            return Task.CompletedTask;
        }

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }

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

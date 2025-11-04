using Spectre.Console;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class VaSettings : FileSettings { }

public class VaAction(Printer printer, IAnsiConsole console, ActionContext<VaSettings> context)
    : IFileAction<VaSettings>
{
    public Task Execute(string file, int index)
    {
        var originalTags = Tagger.ReadTags(file);

        var tags = originalTags.Clone();

        tags.Artist = [.. tags.Artist.Select(NormalizeArtistName)];
        tags.AlbumArtist = [.. tags.AlbumArtist.Select(NormalizeArtistName)];
        tags.Composers = [.. tags.Composers.Select(NormalizeArtistName)];

        if (!ActionHelper.TagDataChanged(originalTags, tags, console))
        {
            context.Skip();
            return Task.CompletedTask;
        }

        printer.PrintTagData(tags);

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

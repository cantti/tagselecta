using System.Globalization;
using System.Text;
using Scriban;
using Scriban.Runtime;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Shared.Tagging;

public class TagDataFormatter
{
    private readonly bool _sanitize;
    private readonly TagDataForTemplate _tagDataForTemplate;

    public TagDataFormatter(TagData tagData, string path, bool sanitize = false)
    {
        _sanitize = sanitize;
        _tagDataForTemplate = new TagDataForTemplate
        {
            Path = S(path),
            FileName = S(Path.GetFileNameWithoutExtension(path)),
            Ext = S(Path.GetExtension(path).TrimStart('.')),
            Album = S(tagData.Album),
            AlbumArtist = S(tagData.AlbumArtist.ToJoined()),
            AlbumArtists = tagData.AlbumArtist.Select(S).ToList(),
            Artist = S(tagData.Artist.ToJoined()),
            Artists = tagData.Artist.Select(S).ToList(),
            Bpm = S(tagData.Bpm),
            CatalogNumber = S(tagData.CatalogNumber),
            Comment = S(tagData.Comment),
            Composer = S(tagData.Composer.ToJoined()),
            Composers = tagData.Composer.Select(S).ToList(),
            Conductor = S(tagData.Conductor),
            Copyright = S(tagData.Copyright),
            Date = S(tagData.Date),
            Disc = S(tagData.Disc),
            DiscTotal = S(tagData.DiscTotal),
            Genre = S(tagData.Genre.ToJoined()),
            Genres = tagData.Genre.Select(S).ToList(),
            Isrc = S(tagData.Isrc),
            Label = S(tagData.Label),
            Publisher = S(tagData.Publisher),
            Title = S(tagData.Title),
            Track = S(tagData.Track),
            TrackTotal = S(tagData.TrackTotal),
            Year = DateTime.TryParseExact(
                tagData.Date,
                ["yyyy", "yyyy-MM-dd", "yyyy/MM/dd"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var d
            )
                ? d.Year.ToString()
                : "",
            Extra = tagData.Extra.ToDictionary(x => x.Key, x => S(x.Text)),
        };
    }

    private string S(string input)
    {
        if (!_sanitize)
        {
            return input;
        }

        var s = input.Replace('/', '_').Replace('\\', '_');
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            sb.Append(invalid.Contains(ch) ? '_' : ch);
        }

        s = sb.ToString();

        // collapse repeated underscores
        while (s.Contains("__", StringComparison.Ordinal))
        {
            s = s.Replace("__", "_", StringComparison.Ordinal);
        }

        return s;
    }

    public string Format(string template)
    {
        MemberRenamerDelegate memberRenamer = member => member.Name.ToLowerInvariant();
        var scriptObject = new ScriptObject();
        scriptObject.Import(typeof(Functions), renamer: memberRenamer);
        scriptObject.Import(_tagDataForTemplate, renamer: memberRenamer);
        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        var parsedTemplate = Template.Parse(template);
        if (parsedTemplate.HasErrors)
        {
            throw new TagSelectaException(parsedTemplate.Messages.ToString());
        }
        var result = parsedTemplate.Render(context);
        return result.Trim();
    }
}

public static class Functions
{
    public static string Pad(string? input, int totalWidth = 2)
    {
        input ??= string.Empty;
        return input.PadLeft(totalWidth, '0');
    }
}

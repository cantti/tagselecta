using TagLib.Flac;
using TagLib.Ogg;

namespace TagSelecta.Tagging;

public class FlacTagDataProcessor(XiphComment tag, Metadata flac) : TagDataProcessor
{
    private readonly XiphComment xiph = tag;
    private readonly Metadata flac = flac;

    private static readonly HashSet<string> _usedXiphFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "album",
        "albumartist",
        "artist",
        "comment",
        "composer",
        "tracknumber",
        "tracktotal",
        "discnumber",
        "disctotal",
        "genre",
        "title",
        "date",
        "label",
        "catalognumber",
        "discogs_release_id",
    };

    public override TagData Read()
    {
        return new TagData
        {
            Album = xiph.Album ?? "",
            AlbumArtists = xiph.GetField("albumartist").ToList(),
            Artists = xiph.Performers.ToList(),
            Comment = xiph.Comment ?? "",
            Composers = xiph.Composers.ToList(),
            Track = ReadField("tracknumber"),
            TrackTotal = ReadField("tracktotal"),
            Disc = ReadField("discnumber"),
            DiscTotal = ReadField("disctotal"),
            Genres = xiph.Genres.ToList(),
            Title = xiph.Title ?? "",
            Date = ReadField("date"),
            Bpm = ReadField("bpm"),
            Label = ReadField("label"),
            CatalogNumber = ReadField("catalognumber"),
            DiscogsReleaseId = ReadField("discogs_release_id"),
            Custom = ReadCustomFields(),
            Pictures = flac.Pictures.Select(x => new TagLib.Picture(x)).ToList(),
        };
    }

    public override void Write(TagData data)
    {
        xiph.Album = data.Album;
        xiph.Comment = data.Comment;
        xiph.Title = data.Title;
        xiph.AlbumArtists = data.AlbumArtists.ToArray();
        xiph.Performers = data.Artists.ToArray();
        xiph.Composers = data.Composers.ToArray();
        xiph.Genres = data.Genres.ToArray();
        // xiph.Track = data.Track;
        // xiph.TrackCount = (uint)data.TrackTotal;
        // xiph.Disc = (uint)data.Disc;
        // xiph.DiscCount = (uint)data.DiscTotal;
        // xiph.Year = (uint)data.Year;
        WriteField("label", data.Label);
        WriteField("catalognumber", data.CatalogNumber);
        WriteField("discogs_release_id", data.DiscogsReleaseId);
        ClearUnusedFields();
        foreach (var field in data.Custom)
        {
            WriteField(field.Key, field.Text);
        }
        flac.Pictures = data.Pictures.Select(p => new TagLib.Picture(p)).ToArray();
    }

    private string ReadField(string key)
    {
        var data = xiph.GetField(key);
        return data != null ? string.Join(";", data) : "";
    }

    private void ClearUnusedFields()
    {
        foreach (var key in xiph)
        {
            if (!_usedXiphFields.Contains(key))
            {
                xiph.RemoveField(key);
            }
        }
    }

    private void WriteField(string key, string? value)
    {
        xiph.SetField(key, value == "" ? [] : [value]);
    }

    private List<CustomField> ReadCustomFields()
    {
        var result = new List<CustomField>();

        foreach (var key in xiph)
        {
            if (_usedXiphFields.Contains(key))
                continue;

            var values = xiph.GetField(key) ?? [];
            result.Add(new CustomField(key, string.Join("; ", values)));
        }

        return result;
    }
}

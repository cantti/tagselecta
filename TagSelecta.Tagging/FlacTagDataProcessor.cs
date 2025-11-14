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
        "album artist",
        "ensemble",
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
            AlbumArtists = xiph.AlbumArtists?.ToList() ?? [],
            Artists = xiph.Performers?.ToList() ?? [],
            Comment = xiph.Comment ?? "",
            Composers = xiph.Composers?.ToList() ?? [],
            Track = (int)xiph.Track,
            TrackTotal = (int)xiph.TrackCount,
            Disc = (int)xiph.Disc,
            DiscTotal = (int)xiph.DiscCount,
            Genres = xiph.Genres?.ToList() ?? [],
            Title = xiph.Title ?? "",
            Year = (int)xiph.Year,
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
        xiph.Track = (uint)data.Track;
        xiph.TrackCount = (uint)data.TrackTotal;
        xiph.Disc = (uint)data.Disc;
        xiph.DiscCount = (uint)data.DiscTotal;
        xiph.Year = (uint)data.Year;
        WriteField("label", data.Label);
        WriteField("catalognumber", data.CatalogNumber);
        WriteField("discogs_release_id", data.DiscogsReleaseId);
        foreach (var field in data.Custom)
        {
            WriteField(field.Key, field.Value);
        }
        flac.Pictures = data.Pictures.Select(p => new TagLib.Picture(p)).ToArray();
    }

    private string ReadField(string key)
    {
        return xiph.GetField(key)?.FirstOrDefault() ?? "";
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

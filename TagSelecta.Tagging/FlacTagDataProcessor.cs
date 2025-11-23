using TagLib.Flac;
using TagLib.Ogg;
using TagSelecta.Shared;

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
        "bpm",
        "catalognumber",
        "comment",
        "composer",
        "conductor",
        "copyright",
        "date",
        "discnumber",
        "disctotal",
        "discogs_release_id",
        "genre",
        "isrc",
        "label",
        "organization",
        "title",
        "tracknumber",
        "tracktotal",
    };

    public override TagData Read()
    {
        return new TagData
        {
            Album = ReadField("album"),
            AlbumArtist = ReadFieldMulti("albumartist"),
            Artist = ReadFieldMulti("artist"),
            Bpm = ReadField("bpm"),
            CatalogNumber = ReadField("catalognumber"),
            Comment = ReadField("comment"),
            Composer = ReadFieldMulti("composer"),
            Conductor = ReadField("conductor"),
            Copyright = ReadField("copyright"),
            Date = ReadField("date"),
            Disc = ReadField("discnumber"),
            DiscTotal = ReadField("disctotal"),
            DiscogsReleaseId = ReadField("discogs_release_id"),
            Genre = ReadFieldMulti("genre"),
            Isrc = ReadField("isrc"),
            Label = ReadField("label"),
            Publisher = ReadField("organization"),
            Title = ReadField("title"),
            Track = ReadField("tracknumber"),
            TrackTotal = ReadField("tracktotal"),
            Picture = flac.Pictures.Select(x => new TagLib.Picture(x)).ToList(),
            Custom = ReadCustomFields(),
        };
    }

    public override void Write(TagData data)
    {
        WriteField("album", data.Album);
        WriteFieldMulti("albumartist", data.AlbumArtist);
        WriteFieldMulti("artist", data.Artist);
        WriteField("bpm", data.Bpm);
        WriteField("catalognumber", data.CatalogNumber);
        WriteField("comment", data.Comment);
        WriteFieldMulti("composer", data.Composer);
        WriteField("conductor", data.Conductor);
        WriteField("copyright", data.Copyright);
        WriteField("date", data.Date);
        WriteField("discnumber", data.Disc);
        WriteField("disctotal", data.DiscTotal);
        WriteField("discogs_release_id", data.DiscogsReleaseId);
        WriteFieldMulti("genre", data.Genre);
        WriteField("isrc", data.Isrc);
        WriteField("label", data.Label);
        WriteField("Publisher", data.Publisher);
        WriteField("title", data.Title);
        WriteField("tracknumber", data.Track);
        WriteField("tracktotal", data.TrackTotal);
        ClearUnusedFields();
        foreach (var field in data.Custom)
        {
            WriteField(field.Key, field.Text);
        }
        flac.Pictures = data.Picture.Select(p => new TagLib.Picture(p)).ToArray();
    }

    private string ReadField(string key)
    {
        var data = xiph.GetField(key);
        return data?.ToJoined() ?? "";
    }

    private List<string> ReadFieldMulti(string key)
    {
        var data = xiph.GetField(key);
        return data?.ToList() ?? [];
    }

    private void WriteField(string key, string value)
    {
        xiph.SetField(key, value == "" ? [] : [value]);
    }

    private void WriteFieldMulti(string key, List<string> value)
    {
        xiph.SetField(key, value.ToArray());
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

    private List<CustomField> ReadCustomFields()
    {
        var result = new List<CustomField>();

        foreach (var key in xiph)
        {
            if (_usedXiphFields.Contains(key))
                continue;

            var values = xiph.GetField(key) ?? [];
            result.Add(new CustomField(key, values.ToJoined()));
        }

        return result;
    }
}

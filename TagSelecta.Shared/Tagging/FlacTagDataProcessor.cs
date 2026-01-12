using TagLib.Flac;
using TagLib.Ogg;

namespace TagSelecta.Shared.Tagging;

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
        var tagData = new TagData
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
            Genre = ReadFieldMulti("genre"),
            Isrc = ReadField("isrc"),
            Label = ReadField("label"),
            Publisher = ReadField("organization"),
            Title = ReadField("title"),
            Track = ReadField("tracknumber"),
            TrackTotal = ReadField("tracktotal"),
            Picture = flac.Pictures.Select(x => new TagLib.Picture(x)).ToList(),
        };
        ReadCustomFields(tagData);
        return tagData;
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
        flac.Pictures = data.Picture.Select(p => new TagLib.Picture(p)).ToArray<TagLib.IPicture>();
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

    private void ReadCustomFields(TagData tagData)
    {
        foreach (var key in xiph)
        {
            var normKey = key.NormalizeKey();

            if (_usedXiphFields.Contains(normKey))
                continue;

            var values = xiph.GetField(key) ?? [];
            tagData.SetCustomField(normKey, values.ToJoined());
        }
    }
}

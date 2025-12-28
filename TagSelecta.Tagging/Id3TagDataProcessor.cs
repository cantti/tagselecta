using TagLib.Id3v2;
using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class Id3TagDataProcessor(Tag tag) : TagDataProcessor
{
    private readonly Tag id3v2 = tag;

    private static readonly HashSet<string> _usedUserTextFields = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "label",
        "catalognumber",
        "discogs_release_id",
    };

    public override TagData Read()
    {
        return new TagData
        {
            Album = id3v2.Album ?? "",
            AlbumArtist = id3v2.AlbumArtists.ToList(),
            Artist = id3v2.Performers.ToList(),
            Bpm = GetText("TBPM"),
            CatalogNumber = GetUserTextAsString("catalognumber"),
            Comment = id3v2.Comment ?? "",
            Composer = id3v2.Composers.ToList(),
            Conductor = id3v2.Conductor ?? "",
            Copyright = id3v2.Copyright ?? "",
            Date = GetText("TDRC"),
            Disc = GetTextValueAndTotal("TPOS").Value,
            DiscTotal = GetTextValueAndTotal("TPOS").Total,
            DiscogsReleaseId = GetUserTextAsString("discogs_release_id"),
            Genre = id3v2.Genres.ToList(),
            Isrc = id3v2.ISRC ?? "",
            Label = GetUserTextAsString("label"),
            Publisher = id3v2.Publisher ?? "",
            Title = id3v2.Title ?? "",
            Track = GetTextValueAndTotal("TRCK").Value,
            TrackTotal = GetTextValueAndTotal("TRCK").Total,
            Picture = id3v2.Pictures.Select(x => new TagLib.Picture(x)).ToList(),
            Custom = ReadCustomFields(),
        };
    }

    public override void Write(TagData data)
    {
        id3v2.Version = 4;
        id3v2.Album = data.Album;
        id3v2.AlbumArtists = data.AlbumArtist.ToArray();
        id3v2.Performers = data.Artist.ToArray();
        WriteText("TBPM", data.Bpm);
        WriteUserText("catalognumber", data.CatalogNumber);
        id3v2.Comment = data.Comment;
        id3v2.Composers = data.Composer.ToArray();
        id3v2.Conductor = data.Conductor;
        id3v2.Copyright = data.Copyright;
        WriteText("TDRC", data.Date);
        WriteTextValueAndTotal("TPOS", data.Disc, data.DiscTotal);
        WriteUserText("discogs_release_id", data.DiscogsReleaseId);
        id3v2.Genres = data.Genre.ToArray();
        id3v2.ISRC = data.Isrc;
        WriteUserText("label", data.Label);
        id3v2.Publisher = data.Publisher;
        id3v2.Title = data.Title;
        WriteTextValueAndTotal("TRCK", data.Track, data.TrackTotal);
        id3v2.Pictures = data.Picture.Select(p => new TagLib.Picture(p)).ToArray<TagLib.IPicture>();
        ClearUnusedUserTextFrames();
        foreach (var field in data.Custom)
        {
            WriteUserText(field.Key, field.Text);
        }
    }

    private List<CustomField> ReadCustomFields()
    {
        var list = new List<CustomField>();

        foreach (var frame in id3v2.GetFrames())
        {
            if (frame is UserTextInformationFrame txxx)
            {
                var key = txxx.Description.NormalizeKey();
                if (_usedUserTextFields.Contains(key))
                {
                    continue;
                }
                var text = txxx.Text.ToJoined();
                var existing = list.SingleOrDefault(x => x.Key == key);
                if (existing != null)
                {
                    existing.Text = $"{existing.Text}; {text}";
                }
                else
                {
                    list.Add(new(key, text));
                }
            }
        }
        return list.OrderBy(x => x.Key).ToList();
    }

    private string GetText(string ident)
    {
        var frame = TextInformationFrame.Get(id3v2, ident, false);
        return frame == null ? "" : frame.Text.ToJoined();
    }

    private void WriteText(string ident, string text)
    {
        id3v2.SetTextFrame(ident, text);
    }

    private (string Value, string Total) GetTextValueAndTotal(string ident)
    {
        var raw = GetText(ident);

        if (string.IsNullOrWhiteSpace(raw))
            return ("", "");

        var parts = raw.Split('/', 2, StringSplitOptions.TrimEntries);

        var value = parts.Length > 0 ? parts[0] : "";
        var total = parts.Length > 1 ? parts[1] : "";

        return (value, total);
    }

    private void WriteTextValueAndTotal(string ident, string value, string total)
    {
        string text = string.IsNullOrEmpty(total) ? value : $"{value}/{total}";

        var frame = TextInformationFrame.Get(id3v2, ident, true);

        if (string.IsNullOrWhiteSpace(text))
            id3v2.RemoveFrame(frame);
        else
            frame.Text = [text];
    }

    private void ClearUnusedUserTextFrames()
    {
        foreach (var frame in id3v2.GetFrames().ToList())
        {
            if (
                frame is UserTextInformationFrame txxx
                && !_usedUserTextFields.Contains(txxx.Description)
            )
            {
                id3v2.RemoveFrame(txxx);
            }
        }
    }

    private string GetUserTextAsString(string key)
    {
        var frame = UserTextInformationFrame.Get(id3v2, key, Tag.DefaultEncoding, false, false);
        //TXXX frames support multivalue strings, join them up and return
        //only the text from the frame.
        return frame?.Text.ToJoined() ?? "";
    }

    private void WriteUserText(string key, string value)
    {
        var frame = UserTextInformationFrame.Get(
            id3v2,
            key,
            Tag.DefaultEncoding,
            true,
            false //taglib uses case sensitive by default
        );
        frame.Text = [value];
        // TagLib does not automatically removes empty user text frames
        if (value == "")
        {
            id3v2.RemoveFrame(frame);
        }
        else
        {
            frame.Text = [value];
        }
    }
}

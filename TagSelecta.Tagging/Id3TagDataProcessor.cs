using TagLib.Id3v2;

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
            AlbumArtists = id3v2.AlbumArtists.ToList(),
            Artists = id3v2.Performers.ToList(),
            Comment = id3v2.Comment ?? "",
            Composers = id3v2.Composers.ToList(),
            Track = GetValueAndTotal("TRCK").Value,
            TrackTotal = GetValueAndTotal("TRCK").Total,
            Disc = GetValueAndTotal("TPOS").Value,
            DiscTotal = GetValueAndTotal("TPOS").Total,
            Genres = id3v2.Genres.ToList(),
            Title = id3v2.Title ?? "",
            Date = GetText("TDRC"),
            Bpm = GetText("TBPM"),
            Conductor = id3v2.Conductor,
            Custom = ReadCustomFields(),
            Label = GetUserTextAsString("label"),
            CatalogNumber = GetUserTextAsString("catalognumber"),
            DiscogsReleaseId = GetUserTextAsString("discogs_release_id"),
            Pictures = id3v2.Pictures.Select(x => new TagLib.Picture(x)).ToList(),
        };
    }

    public override void Write(TagData data)
    {
        id3v2.Album = data.Album;
        id3v2.Comment = data.Comment;
        id3v2.Title = data.Title;
        id3v2.AlbumArtists = data.AlbumArtists.ToArray();
        id3v2.Performers = data.Artists.ToArray();
        id3v2.Composers = data.Composers.ToArray();
        id3v2.Genres = data.Genres.ToArray();
        WriteValueAndTotal("TRCK", data.Track, data.TrackTotal);
        WriteValueAndTotal("TPOS", data.Disc, data.DiscTotal);
        WriteText("TDRC", data.Date);
        WriteText("TBPM", data.Bpm);
        WriteUserText("label", data.Label);
        WriteUserText("catalognumber", data.CatalogNumber);
        WriteUserText("discogs_release_id", data.DiscogsReleaseId);
        ClearUnusedUserTextFrames();
        foreach (var field in data.Custom)
        {
            WriteUserText(field.Key, field.Value);
        }
        id3v2.Pictures = data.Pictures.Select(p => new TagLib.Picture(p)).ToArray();
    }

    private List<CustomField> ReadCustomFields()
    {
        var list = new List<CustomField>();

        foreach (var frame in id3v2.GetFrames())
        {
            if (
                frame is UserTextInformationFrame txxx
                && !_usedUserTextFields.Contains(txxx.Description)
            )
            {
                // todo handle duplicated txxx
                list.Add(new CustomField(txxx.Description, string.Join("; ", txxx.Text)));
            }
        }
        return list;
    }

    private string GetUserTextAsString(string key)
    {
        var frame = UserTextInformationFrame.Get(id3v2, key, Tag.DefaultEncoding, false, false);
        //TXXX frames support multivalue strings, join them up and return
        //only the text from the frame.
        var result = frame == null ? null : string.Join(";", frame.Text);
        return string.IsNullOrEmpty(result) ? "" : result;
    }

    private string GetText(string ident)
    {
        return id3v2.GetTextAsString(ident);
    }

    private void WriteText(string ident, string text)
    {
        id3v2.SetTextFrame(ident, text);
    }

    private (string Value, string Total) GetValueAndTotal(string ident)
    {
        var raw = GetText(ident);

        if (string.IsNullOrWhiteSpace(raw))
            return ("", "");

        var parts = raw.Split('/', 2, StringSplitOptions.TrimEntries);

        var value = parts.Length > 0 ? parts[0] : "";
        var total = parts.Length > 1 ? parts[1] : "";

        return (value, total);
    }

    private void WriteValueAndTotal(string ident, string value, string total)
    {
        string text = string.IsNullOrEmpty(total) ? value : $"{value}/{total}";

        var frame = TextInformationFrame.Get(id3v2, ident, true);

        if (string.IsNullOrWhiteSpace(text))
            id3v2.RemoveFrame(frame);
        else
            frame.Text = [text];
    }

    private List<string> GetTextAsList(string ident)
    {
        var frame = TextInformationFrame.Get(id3v2, ident, false);
        return frame != null ? frame.Text.ToList() : [];
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

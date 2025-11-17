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
            // 1. Album
            Album = id3v2.Album ?? "",

            // 2. AlbumArtists
            AlbumArtists = id3v2.AlbumArtists.ToList(),

            // 3. Artists
            Artists = id3v2.Performers.ToList(),

            // 4. Bpm
            Bpm = GetText("TBPM"),

            // 5. CatalogNumber
            CatalogNumber = GetUserTextAsString("catalognumber"),

            // 6. Comment
            Comment = id3v2.Comment ?? "",

            // 7. Composers
            Composers = id3v2.Composers.ToList(),

            // 8. Conductor
            Conductor = id3v2.Conductor,

            // 9. Date
            Date = GetText("TDRC"),

            // 10–11. Disc + DiscTotal
            Disc = GetTextValueAndTotal("TPOS").Value,
            DiscTotal = GetTextValueAndTotal("TPOS").Total,

            // 12. DiscogsReleaseId
            DiscogsReleaseId = GetUserTextAsString("discogs_release_id"),

            // 13. Genres
            Genres = id3v2.Genres.ToList(),

            // 14. Isrc
            Isrc = id3v2.ISRC,

            // 15. Label
            Label = GetUserTextAsString("label"),

            // 16. Title
            Title = id3v2.Title ?? "",

            // 17–18. Track + TrackTotal
            Track = GetTextValueAndTotal("TRCK").Value,
            TrackTotal = GetTextValueAndTotal("TRCK").Total,

            // Pictures (not numbered)
            Pictures = id3v2.Pictures.Select(x => new TagLib.Picture(x)).ToList(),

            // Custom (not numbered)
            Custom = ReadCustomFields(),
        };
    }

    public override void Write(TagData data)
    {
        id3v2.Version = 4;

        // 1. Album
        id3v2.Album = data.Album;

        // 2. AlbumArtists
        id3v2.AlbumArtists = data.AlbumArtists.ToArray();

        // 3. Artists
        id3v2.Performers = data.Artists.ToArray();

        // 4. Bpm
        WriteText("TBPM", data.Bpm);

        // 5. CatalogNumber
        WriteUserText("catalognumber", data.CatalogNumber);

        // 6. Comment
        id3v2.Comment = data.Comment;

        // 7. Composers
        id3v2.Composers = data.Composers.ToArray();

        // 8. Conductor
        id3v2.Conductor = data.Conductor;

        // 9. Date
        WriteText("TDRC", data.Date);

        // 10–11. Disc + DiscTotal
        WriteTextValueAndTotal("TPOS", data.Disc, data.DiscTotal);

        // 12. DiscogsReleaseId
        WriteUserText("discogs_release_id", data.DiscogsReleaseId);

        // 13. Genres
        id3v2.Genres = data.Genres.ToArray();

        // 14. Isrc
        id3v2.ISRC = data.Isrc;

        // 15. Label
        WriteUserText("label", data.Label);

        // 16. Title
        id3v2.Title = data.Title;

        // 17–18. Track + TrackTotal
        WriteTextValueAndTotal("TRCK", data.Track, data.TrackTotal);

        // Pictures (not numbered)
        id3v2.Pictures = data.Pictures.Select(p => new TagLib.Picture(p)).ToArray();

        // Custom (not numbered)
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
            if (
                frame is UserTextInformationFrame txxx
                && !_usedUserTextFields.Contains(txxx.Description)
            )
            {
                var description = txxx.Description?.ToLowerInvariant() ?? "";
                var text = txxx.Text.ToJoined();
                var existing = list.SingleOrDefault(x => x.Key == description);
                if (existing != null)
                {
                    existing.Text = $"{existing.Text}; {text}";
                }
                else
                {
                    list.Add(new(description, text));
                }
            }
        }
        return list;
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
        var result = frame == null ? null : string.Join(";", frame.Text);
        return string.IsNullOrEmpty(result) ? "" : result;
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

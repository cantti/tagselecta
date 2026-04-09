using TagLib;
using TagLib.Id3v2;
using Tag = TagLib.Id3v2.Tag;

namespace TagSelecta.Shared.Tagging;

public class Id3TagDataProcessor(Tag id3v2) : TagDataProcessor
{
    public override TagData Read()
    {
        var tagData = new TagData();

        var disc = GetTextValueAndTotal("TPOS");
        var track = GetTextValueAndTotal("TRCK");

        tagData.SetValue(FieldName.Album, id3v2.Album);
        tagData.SetValue(FieldName.AlbumArtist, id3v2.AlbumArtists);
        tagData.SetValue(FieldName.Artist, id3v2.Performers);
        tagData.SetValue(FieldName.Bpm, GetText("TBPM"));
        tagData.SetValue(FieldName.Comment, id3v2.Comment);
        tagData.SetValue(FieldName.Composer, id3v2.Composers);
        tagData.SetValue(FieldName.Conductor, id3v2.Conductor);
        tagData.SetValue(FieldName.Copyright, id3v2.Copyright);
        tagData.SetValue(FieldName.Date, GetText("TDRC"));
        tagData.SetValue(FieldName.Disc, disc.Value);
        tagData.SetValue(FieldName.DiscTotal, disc.Total);
        tagData.SetValue(FieldName.Genre, id3v2.Genres);
        tagData.SetValue(FieldName.Isrc, id3v2.ISRC);
        tagData.SetValue(FieldName.Publisher, id3v2.Publisher);
        tagData.SetValue(FieldName.Title, id3v2.Title);
        tagData.SetValue(FieldName.Track, track.Value);
        tagData.SetValue(FieldName.TrackTotal, track.Total);

        var userTextFields = id3v2.GetFrames<UserTextInformationFrame>();

        foreach (var userTextField in userTextFields)
        {
            var key = userTextField.Description.NormalizeKey();

            // normal text frames take precedence over user text frames
            if (FieldName.All().Contains(key))
            {
                continue;
            }

            tagData.SetValue(key, userTextField.Text.JoinTagValues());
        }

        tagData.Picture = id3v2.Pictures.Select(x => new Picture(x)).ToList();
        return tagData;
    }

    public override void Write(TagData data)
    {
        id3v2.Version = 4;
        id3v2.Album = data.GetValueFirst(FieldName.Album);
        id3v2.AlbumArtists = data.GetValue(FieldName.AlbumArtist).ToArray();
        id3v2.Performers = data.GetValue(FieldName.Artist).ToArray();
        WriteText("TBPM", data.GetValueFirst(FieldName.Bpm));
        id3v2.Comment = data.GetValueFirst(FieldName.Comment);
        id3v2.Composers = data.GetValue(FieldName.Composer).ToArray();
        id3v2.Conductor = data.GetValueFirst(FieldName.Conductor);
        id3v2.Copyright = data.GetValueFirst(FieldName.Copyright);
        WriteText("TDRC", data.GetValueFirst(FieldName.Date));
        WriteTextValueAndTotal(
            "TPOS",
            data.GetValueFirst(FieldName.Disc),
            data.GetValueFirst(FieldName.DiscTotal)
        );
        id3v2.Genres = data.GetValue(FieldName.Genre).ToArray();
        id3v2.ISRC = data.GetValueFirst(FieldName.Isrc);
        id3v2.Publisher = data.GetValueFirst(FieldName.Publisher);
        id3v2.Title = data.GetValueFirst(FieldName.Title);
        WriteTextValueAndTotal(
            "TRCK",
            data.GetValueFirst(FieldName.Track),
            data.GetValueFirst(FieldName.TrackTotal)
        );
        id3v2.Pictures = data.Picture.Select(p => new Picture(p)).ToArray<IPicture>();
        ClearUserTextFrames();
        foreach (var field in data.Fields.Where(f => !FieldName.All().Contains(f.Key)))
        {
            WriteUserText(field.Key, field.Text.JoinTagValues());
        }
    }

    private string GetText(string ident)
    {
        var frame = TextInformationFrame.Get(id3v2, ident, false);
        return frame == null ? "" : frame.Text.JoinTagValues();
    }

    private void WriteText(string ident, string text)
    {
        id3v2.SetTextFrame(ident, text);
    }

    private (string Value, string Total) GetTextValueAndTotal(string ident)
    {
        var raw = GetText(ident);

        if (string.IsNullOrWhiteSpace(raw))
        {
            return ("", "");
        }

        var parts = raw.Split('/', 2, StringSplitOptions.TrimEntries);

        var value = parts.Length > 0 ? parts[0] : "";
        var total = parts.Length > 1 ? parts[1] : "";

        return (value, total);
    }

    private void WriteTextValueAndTotal(string ident, string value, string total)
    {
        var text = string.IsNullOrEmpty(total) ? value : $"{value}/{total}";

        var frame = TextInformationFrame.Get(id3v2, ident, true);

        if (string.IsNullOrWhiteSpace(text))
        {
            id3v2.RemoveFrame(frame);
        }
        else
        {
            frame.Text = [text];
        }
    }

    private void ClearUserTextFrames()
    {
        foreach (var frame in id3v2.GetFrames().ToList())
        {
            if (frame is UserTextInformationFrame txxx)
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

using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;

namespace TagSelecta.Shared.Tagging;

public class Id3TagDataProcessor : TagDataProcessor
{
    private readonly File _tfile;
    private readonly TagLib.Id3v2.Tag id3v2;

    public Id3TagDataProcessor(File tfile)
    {
        _tfile = tfile;
        id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagTypes.Id3v2, true);
    }

    public override TagData Read()
    {
        var tagData = new TagData();

        var disc = GetTextValueAndTotal("TPOS");
        var track = GetTextValueAndTotal("TRCK");

        tagData.SetValue(FieldName.Album, ReadValue("TALB"));
        tagData.SetValue(FieldName.AlbumArtist, ReadValue("TPE2"));
        tagData.SetValue(FieldName.Artist, ReadValue("TPE1"));
        tagData.SetValue(FieldName.Bpm, ReadValue("TBPM"));
        tagData.SetValue(FieldName.Comment, id3v2.Comment);
        tagData.SetValue(FieldName.Composer, ReadValue("TCOM"));
        tagData.SetValue(FieldName.Conductor, ReadValue("TPE3"));
        tagData.SetValue(FieldName.Copyright, ReadValue("TCOP"));
        tagData.SetValue(FieldName.Date, ReadValue("TDRC"));
        tagData.SetValue(FieldName.DiscNumber, disc.Value);
        tagData.SetValue(FieldName.DiscTotal, disc.Total);
        tagData.SetValue(FieldName.Genre, ReadValue("TCON"));
        tagData.SetValue(FieldName.Isrc, ReadValue("TSRC"));
        tagData.SetValue(FieldName.Publisher, ReadValue("TPUB"));
        tagData.SetValue(FieldName.Title, ReadValue("TIT2"));
        tagData.SetValue(FieldName.TrackNumber, track.Value);
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
        WriteValue("TALB", data.GetValue(FieldName.Album));
        WriteValue("TPE2", data.GetValue(FieldName.AlbumArtist));
        WriteValue("TPE1", data.GetValue(FieldName.Artist));
        WriteValue("TBPM", data.GetValue(FieldName.Bpm));
        id3v2.Comment = data.GetValueFirst(FieldName.Comment);
        WriteValue("TCOM", data.GetValue(FieldName.Composer));
        WriteValue("TPE3", data.GetValue(FieldName.Conductor));
        WriteValue("TCOP", data.GetValue(FieldName.Copyright));
        WriteValue("TDRC", data.GetValue(FieldName.Date));
        WriteValueWithTotal(
            "TPOS",
            data.GetValueFirst(FieldName.DiscNumber),
            data.GetValueFirst(FieldName.DiscTotal)
        );
        WriteValue("TCON", data.GetValue(FieldName.Genre));
        WriteValue("TSRC", data.GetValue(FieldName.Isrc));
        WriteValue("TPUB", data.GetValue(FieldName.Publisher));
        WriteValue("TIT2", data.GetValue(FieldName.Title));
        WriteValueWithTotal(
            "TRCK",
            data.GetValueFirst(FieldName.TrackNumber),
            data.GetValueFirst(FieldName.TrackTotal)
        );
        id3v2.Pictures = data.Picture.Select(p => new Picture(p)).ToArray<IPicture>();
        ClearUserTextFrames();
        foreach (var field in data.Fields.Where(f => !FieldName.All().Contains(f.Key)))
        {
            WriteUserText(field.Key, field.Text.JoinTagValues());
        }

        FixId3V1(data);
    }

    private void FixId3V1(TagData data)
    {
        // do nothing if there is no id3v1 tag
        if (!_tfile.TagTypes.HasFlag(TagTypes.Id3v1))
        {
            return;
        }

        // always remove old id3v1 tag first
        _tfile.RemoveTags(TagTypes.Id3v1);

        // if user removed id3v1 tag that is enough
        if (!data.Tags.Any(x => x.Equals(nameof(TagTypes.Id3v1), StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        // recreate id3v1 tag otherwise. Taglib will populate id3v1 tag with data from id3v2 tag
        var id3v1 = (TagLib.Id3v1.Tag)_tfile.GetTag(TagTypes.Id3v1, true);
    }

    private List<string> ReadValue(string ident)
    {
        var frame = TextInformationFrame.Get(id3v2, ident, false);
        return frame == null ? [] : frame.Text.ToList();
    }

    private void WriteValue(string ident, List<string> text)
    {
        id3v2.SetTextFrame(ident, text.ToArray());
    }

    private (string Value, string Total) GetTextValueAndTotal(string ident)
    {
        var raw = ReadValue(ident).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(raw))
        {
            return ("", "");
        }

        var parts = raw.Split('/', 2, StringSplitOptions.TrimEntries);

        var value = parts.Length > 0 ? parts[0] : "";
        var total = parts.Length > 1 ? parts[1] : "";

        return (value, total);
    }

    private void WriteValueWithTotal(string ident, string value, string total)
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
            TagLib.Id3v2.Tag.DefaultEncoding,
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

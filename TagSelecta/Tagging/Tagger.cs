namespace TagSelecta.Tagging;

public static class Tagger
{
    private static readonly string[] allowedMime = ["taglib/mp3", "taglib/flac", "taglib/ogg"];

    public static TagData ReadTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        if (!allowedMime.Contains(tfile.MimeType))
        {
            throw new ActionException("Invalid file type");
        }
        var tag = tfile.Tag;
        var tagData = TagLibToTagDataMapper.Map(tag);
        tagData.Path = file;
        tagData.Label = GetExtValue(tfile, "label");
        tagData.CatalogNumber = GetExtValue(tfile, "catalognumber");
        tagData.CatalogNumber = GetExtValue(tfile, "catalognumber");
        tagData.DiscogsReleaseId = GetExtValue(tfile, "discogs_release_id");
        return tagData;
    }

    public static void WriteTags(string file, TagData tagData)
    {
        using var tfile = TagLib.File.Create(file);
        if (!allowedMime.Contains(tfile.MimeType))
        {
            throw new Exception("Invalid file type");
        }

        // todo use mapperly
        var tag = tfile.Tag;
        TagDataToTagLibMapper.Map(tagData, tag);
        SetExtValue(tfile, "label", tagData.Label);
        SetExtValue(tfile, "catalognumber", tagData.CatalogNumber);
        SetExtValue(tfile, "discogs_release_id", tagData.DiscogsReleaseId);
        tfile.Save();
    }

    private static string GetExtValue(TagLib.File tfile, string key)
    {
        string mime = tfile.MimeType.ToLowerInvariant();
        if (mime == "taglib/mp3")
        {
            return GetMp3ExtValue(tfile, key);
        }
        else if (mime == "taglib/flac" || mime == "taglib/ogg")
        {
            return GetFlacExtValue(tfile, key);
        }
        else
        {
            return "";
        }
    }

    private static string GetMp3ExtValue(TagLib.File tfile, string key)
    {
        var id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2, false);
        var frame = TagLib.Id3v2.UserTextInformationFrame.Get(id3v2, key, create: false);
        return frame?.Text.FirstOrDefault() ?? "";
    }

    private static string GetFlacExtValue(TagLib.File tfile, string key)
    {
        var xiph = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph, false);
        return xiph?.GetField(key).FirstOrDefault() ?? "";
    }

    private static void SetExtValue(TagLib.File tfile, string key, string value)
    {
        string mime = tfile.MimeType.ToLowerInvariant();
        if (mime == "taglib/mp3")
        {
            SetMp3ExtValue(tfile, key, value);
        }
        else if (mime == "taglib/flac" || mime == "taglib/ogg")
        {
            SetFlacExtValue(tfile, key, value);
        }
    }

    private static void SetFlacExtValue(TagLib.File tfile, string key, string value)
    {
        var xiph = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph, true);
        // TagLib automatically removes fields that are empty arrays
        xiph.SetField(key, value == "" ? [] : [value]);
    }

    private static void SetMp3ExtValue(TagLib.File tfile, string key, string value)
    {
        var id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2, true);
        var frame = TagLib.Id3v2.UserTextInformationFrame.Get(id3v2, key, true);
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

    public static void RemoveTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        tfile.RemoveTags(TagLib.TagTypes.AllTags);
        tfile.Save();
    }
}

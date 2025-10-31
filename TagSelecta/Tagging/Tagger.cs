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
        return new TagData
        {
            Album = tag.Album ?? "",
            Artist = [.. tag.Performers],
            AlbumArtist = [.. tag.AlbumArtists],
            Title = tag.Title ?? "",
            Genre = [.. tag.Genres],
            Year = tag.Year,
            Track = tag.Track,
            TrackTotal = tag.TrackCount,
            Disc = tag.Disc,
            DiscTotal = tag.DiscCount,
            Comment = tag.Comment ?? "",
            Label = GetExtValue(tfile, "label"),
            CatalogNumber = GetExtValue(tfile, "catalognumber"),
            Pictures = [.. tag.Pictures],
            Bpm = tag.BeatsPerMinute,
        };
    }

    public static void WriteTags(string file, TagData tagData)
    {
        using var tfile = TagLib.File.Create(file);
        if (!allowedMime.Contains(tfile.MimeType))
        {
            throw new Exception("Invalid file type");
        }

        var tag = tfile.Tag;
        tag.Album = tagData.Album;
        tag.Performers = [.. tagData.Artist];
        tag.AlbumArtists = [.. tagData.AlbumArtist];
        tag.Title = tagData.Title;
        tag.Genres = [.. tagData.Genre];
        tag.Year = tagData.Year;
        tag.Track = tagData.Track;
        tag.TrackCount = tagData.TrackTotal;
        tag.Disc = tagData.Disc;
        tag.DiscCount = tagData.DiscTotal;
        tag.Comment = tagData.Comment;
        tag.Pictures = [.. tagData.Pictures];
        tag.BeatsPerMinute = tagData.Bpm;
        SetExtValue(tfile, "label", tagData.Label);
        SetExtValue(tfile, "catalognumber", tagData.CatalogNumber);
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

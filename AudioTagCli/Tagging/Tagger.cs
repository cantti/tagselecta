namespace AudioTagCli.Tagging;

public static class Tagger
{
    private static readonly string[] allowedMime = ["taglib/mp3", "taglib/flac", "taglib/ogg"];

    public static TagData ReadTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        if (!allowedMime.Contains(tfile.MimeType))
        {
            throw new AudioTagHelperException("Invalid file type");
        }
        var tag = tfile.Tag;
        return new TagData
        {
            Path = file,
            Album = tag.Album,
            Artist = [.. tag.Performers],
            AlbumArtist = [.. tag.AlbumArtists],
            Title = tag.Title,
            Genre = [.. tag.Genres],
            Year = tag.Year,
            Track = tag.Track,
            TrackTotal = tag.TrackCount,
            Disc = tag.Disc,
            DiscTotal = tag.DiscCount,
            Comments = tag.Comment,
            Label = GetExtValue(tfile, "label"),
            CatalogNumber = GetExtValue(tfile, "catalognumber"),
            Pictures = [.. tag.Pictures],
        };
    }

    public static void WriteTags(string file, TagData tagData)
    {
        using var tfile = TagLib.File.Create(file);
        if (!allowedMime.Contains(tfile.MimeType))
        {
            throw new Exception("Invalid file type");
        }

        var commonTags = tfile.Tag;
        commonTags.Album = tagData.Album;
        commonTags.Performers = [.. tagData.Artist];
        commonTags.AlbumArtists = [.. tagData.AlbumArtist];
        commonTags.Title = tagData.Title;
        commonTags.Genres = [.. tagData.Genre];
        commonTags.Year = tagData.Year;
        commonTags.Track = tagData.Track;
        commonTags.TrackCount = tagData.TrackTotal;
        commonTags.Disc = tagData.Disc;
        commonTags.DiscCount = tagData.DiscTotal;
        commonTags.Comment = tagData.Comments;
        commonTags.Pictures = [.. tagData.Pictures];
        SetExtValue(tfile, "label", tagData.Label);
        tfile.Save();
    }

    private static string? GetExtValue(TagLib.File tfile, string key)
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
            return null;
        }
    }

    private static string? GetMp3ExtValue(TagLib.File tfile, string key)
    {
        var id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2, false);
        var frame = TagLib.Id3v2.UserTextInformationFrame.Get(id3v2, key, create: false);
        return frame?.Text.FirstOrDefault();
    }

    private static string? GetFlacExtValue(TagLib.File tfile, string key)
    {
        var xiph = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph, false);
        return xiph?.GetField(key).FirstOrDefault();
    }

    private static void SetExtValue(TagLib.File tfile, string key, string? value)
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

    private static void SetFlacExtValue(TagLib.File tfile, string key, string? value)
    {
        var xiph = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph, true);
        xiph.SetField(key, [value]);
    }

    private static void SetMp3ExtValue(TagLib.File tfile, string key, string? value)
    {
        var id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2, true);
        var frame = TagLib.Id3v2.UserTextInformationFrame.Get(id3v2, key, true);
        frame.Text = [value];
    }

    private static void RemoveTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        tfile.RemoveTags(TagLib.TagTypes.AllTags);
        tfile.Save();
    }
}

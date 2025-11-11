using System.Reflection;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Tagging;

public static class Tagger
{
    private static readonly string[] allowedMime = ["taglib/mp3", "taglib/flac", "taglib/ogg"];

    public static TagData ReadTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        if (!allowedMime.Contains(tfile.MimeType))
        {
            throw new TagSelectaException("Invalid file type");
        }
        var tag = tfile.Tag;
        var tagData = new TagData
        {
            Album = tag.Album,
            AlbumArtists = [.. tag.AlbumArtists],
            Artists = [.. tag.Performers],
            Comment = tag.Comment,
            Composers = [.. tag.Composers],
            Track = tag.Track,
            TrackTotal = tag.TrackCount,
            Disc = tag.Disc,
            DiscTotal = tag.DiscCount,
            Genres = [.. tag.Genres],
            Title = tag.Title,
            Year = tag.Year,
            Path = file,
            Pictures = [.. tag.Pictures.Select(x => new TagLib.Picture(x))],
            Custom = [.. GetAllExtValues(tfile).OrderBy(x => x.Key)],
        };
        return tagData;
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
        tag.AlbumArtists = [.. tagData.AlbumArtists];
        tag.Performers = [.. tagData.Artists];
        tag.Comment = tagData.Comment;
        tag.Composers = [.. tagData.Composers];
        tag.Track = tagData.Track;
        tag.TrackCount = tagData.TrackTotal;
        tag.Disc = tagData.Disc;
        tag.DiscCount = tagData.DiscTotal;
        tag.Genres = [.. tagData.Genres];
        tag.Title = tagData.Title;
        tag.Year = tagData.Year;
        tag.Pictures = [.. tag.Pictures];
        foreach (var extraField in tagData.Custom)
        {
            SetExtValue(tfile, extraField.Key, extraField.Value);
        }
        tfile.Save();
    }

    private static List<CustomTag> GetAllExtValues(TagLib.File tfile)
    {
        string mime = tfile.MimeType.ToLowerInvariant();
        if (mime == "taglib/mp3")
        {
            return GetAllMp3ExtValues(tfile);
        }
        else if (mime == "taglib/flac" || mime == "taglib/ogg")
        {
            return GetAllFlacExtValues(tfile);
        }
        else
        {
            throw new TagSelectaException("Invalid type");
        }
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

    private static List<CustomTag> GetAllMp3ExtValues(TagLib.File tfile)
    {
        var id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2, false);
        var result = new List<CustomTag>();

        if (id3v2 == null)
            return result;

        var frames = id3v2.GetFrames<TagLib.Id3v2.UserTextInformationFrame>();

        foreach (var frame in frames)
        {
            // todo: review implementation, considering mp3 tags case sensitive
            var key = frame.Description;
            var value = frame.Text?.FirstOrDefault() ?? "";
            result.Add(new CustomTag(key, value));
        }

        return result;
    }

    private static List<CustomTag> GetAllFlacExtValues(TagLib.File tfile)
    {
        var xiph = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph, false);
        var result = new List<CustomTag>();

        if (xiph == null)
            return result;

        var excluded = typeof(TagData)
            .GetProperties()
            .Select(p => p.GetCustomAttribute<XiphKeyAttribute>())
            .Where(a => a != null)
            .SelectMany(a => a!.Keys)
            .Select(x => x.ToLower())
            .ToArray();

        foreach (var key in xiph)
        {
            if (excluded.Contains(key, StringComparer.OrdinalIgnoreCase))
                continue;

            var value = xiph.GetField(key)?.FirstOrDefault() ?? "";
            result.Add(new CustomTag(key, value));
        }

        return result;
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
        var frame = TagLib.Id3v2.UserTextInformationFrame.Get(
            id3v2,
            key,
            TagLib.Id3v2.Tag.DefaultEncoding,
            false,
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

    public static void RemoveTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        tfile.RemoveTags(TagLib.TagTypes.AllTags);
        tfile.Save();
    }
}

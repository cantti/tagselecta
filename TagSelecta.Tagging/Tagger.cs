using TagLib;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Tagging;

public static class Tagger
{
    public static TagData ReadTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        var processor = CreateProcessor(tfile);
        var tagData = processor.Read();
        return tagData;
    }

    public static void WriteTags(string file, TagData data)
    {
        using var tfile = TagLib.File.Create(file);
        var processor = CreateProcessor(tfile);
        processor.Write(data);
        tfile.Save();
    }

    public static void RemoveTags(string file)
    {
        using var tfile = TagLib.File.Create(file);
        tfile.RemoveTags(TagTypes.AllTags);
        tfile.Save();
    }

    private static TagDataProcessor CreateProcessor(TagLib.File tfile)
    {
        string mime = tfile.MimeType.ToLowerInvariant();
        if (mime.Contains("flac"))
        {
            var xiph = (TagLib.Ogg.XiphComment)tfile.GetTag(TagTypes.Xiph, true);
            var flac = (TagLib.Flac.Metadata)tfile.GetTag(TagTypes.FlacMetadata, true);
            return new FlacTagDataProcessor(xiph, flac);
        }
        if (mime.Contains("mpeg") || mime.Contains("mp3"))
        {
            var id3v2 = (TagLib.Id3v2.Tag)tfile.GetTag(TagTypes.Id3v2, true);
            id3v2.Version = 4;
            return new Id3TagDataProcessor(id3v2);
        }
        throw new TagSelectaException($"Unsupported format: {mime}");
    }
}

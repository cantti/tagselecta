using TagLib;
using TagLib.Flac;
using TagLib.Ogg;
using TagSelecta.Shared.Exceptions;
using File = TagLib.File;
using Tag = TagLib.Id3v2.Tag;

namespace TagSelecta.Shared.Tagging;

public class Tagger : ITagger
{
    public TagData ReadTags(string file)
    {
        using var tfile = File.Create(file);
        var processor = CreateProcessor(tfile);
        var tagData = processor.Read();
        return tagData;
    }

    public void WriteTags(string file, TagData data)
    {
        using var tfile = File.Create(file);
        var processor = CreateProcessor(tfile);
        processor.Write(data);
        tfile.Save();
    }

    public void RemoveTags(string file)
    {
        using var tfile = File.Create(file);
        tfile.RemoveTags(TagTypes.AllTags);
        tfile.Save();
    }

    private static TagDataProcessor CreateProcessor(File tfile)
    {
        var mime = tfile.MimeType.ToLowerInvariant();
        if (mime.Contains("mpeg") || mime.Contains("mp3") || mime.Contains("wav"))
        {
            var id3v2 = (Tag)tfile.GetTag(TagTypes.Id3v2, true);
            return new Id3TagDataProcessor(id3v2);
        }

        if (mime.Contains("flac"))
        {
            var xiph = (XiphComment)tfile.GetTag(TagTypes.Xiph, true);
            var flac = (Metadata)tfile.GetTag(TagTypes.FlacMetadata, true);
            return new FlacOggTagDataProcessor(xiph, flac);
        }

        if (mime.Contains("ogg"))
        {
            var xiph = (XiphComment)tfile.GetTag(TagTypes.Xiph, true);
            return new FlacOggTagDataProcessor(xiph);
        }

        throw new TagSelectaException($"Unsupported format: {mime}");
    }
}

using TagLib;
using TagSelecta.Shared.Exceptions;
using File = TagLib.File;

namespace TagSelecta.Shared.Tagging;

public class Tagger : ITagger
{
    public TagData ReadTags(string file)
    {
        using var tfile = File.Create(file);
        // taglib create id3v1 tag if it doesn't exist
        // so we need to remove tags not on disk
        tfile.RemoveTags(tfile.TagTypes & ~tfile.TagTypesOnDisk);
        var processor = CreateProcessor(tfile);
        var tagData = processor.Read();
        tagData.Tags = Enum.GetValues<TagTypes>()
            .Where(x =>
                x != TagTypes.None
                && x != TagTypes.AllTags
                && (int)x > 0
                && ((int)x & ((int)x - 1)) == 0
                && tfile.TagTypes.HasFlag(x)
            )
            .Select(x => x.ToString())
            .ToList();
        return tagData;
    }

    public void WriteTags(string file, TagData data)
    {
        using var tfile = File.Create(file);
        var processor = CreateProcessor(tfile);
        processor.Write(data);
        tfile.Save();
    }

    private static TagDataProcessor CreateProcessor(File tfile)
    {
        var mime = tfile.MimeType.ToLowerInvariant();
        if (mime.Contains("mpeg") || mime.Contains("mp3") || mime.Contains("wav"))
        {
            return new Id3TagDataProcessor(tfile);
        }

        if (mime.Contains("flac") || mime.Contains("ogg"))
        {
            return new FlacOggTagDataProcessor(tfile);
        }

        throw new TagSelectaException($"Unsupported format: {mime}");
    }
}

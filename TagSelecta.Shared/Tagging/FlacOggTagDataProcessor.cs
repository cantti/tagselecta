using TagLib;
using TagLib.Flac;
using TagLib.Ogg;
using File = TagLib.File;
using Picture = TagLib.Picture;

namespace TagSelecta.Shared.Tagging;

public class FlacOggTagDataProcessor : TagDataProcessor
{
    private readonly Metadata? _flac;
    private readonly XiphComment _xiph;

    public FlacOggTagDataProcessor(File tfile)
    {
        var mime = tfile.MimeType.ToLowerInvariant();
        _xiph = (XiphComment)tfile.GetTag(TagTypes.Xiph, true);
        if (mime.Contains("flac"))
        {
            _flac = (Metadata)tfile.GetTag(TagTypes.FlacMetadata, true);
        }
    }

    public override TagData Read()
    {
        var tagData = new TagData();

        foreach (var key in _xiph)
        {
            var normKey = key.NormalizeKey();

            if (normKey == "metadata_block_picture")
            {
                continue;
            }

            var values = _xiph.GetField(key) ?? [];
            tagData.SetValue(normKey, values.ToList());
        }

        var pictures = _flac is not null ? _flac.Pictures : _xiph.Pictures;
        tagData.Picture = pictures.Select(x => new Picture(x)).ToList();
        return tagData;
    }

    public override void Write(TagData data)
    {
        foreach (var key in _xiph)
        {
            _xiph.RemoveField(key);
        }

        foreach (var field in data.Fields)
        {
            _xiph.SetField(field.Key, field.Text.ToArray());
        }

        var pictures = data.Picture.Select(p => new Picture(p)).ToArray<IPicture>();
        if (_flac is not null)
        {
            _flac.Pictures = pictures;
        }
        else
        {
            _xiph.Pictures = pictures;
        }
    }
}

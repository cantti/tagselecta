using TagLib;
using TagLib.Flac;
using TagLib.Ogg;
using Picture = TagLib.Picture;

namespace TagSelecta.Shared.Tagging;

public class FlacOggTagDataProcessor : TagDataProcessor
{
    private readonly Metadata? flac;
    private readonly XiphComment xiph;

    public FlacOggTagDataProcessor(XiphComment xiph, Metadata flac)
    {
        this.flac = flac;
        this.xiph = xiph;
    }

    public FlacOggTagDataProcessor(XiphComment xiph)
    {
        this.xiph = xiph;
    }

    public override TagData Read()
    {
        var tagData = new TagData();

        foreach (var key in xiph)
        {
            var normKey = key.NormalizeKey();

            if (normKey == "metadata_block_picture")
            {
                continue;
            }

            var values = xiph.GetField(key) ?? [];
            tagData.SetValue(normKey, values.ToList());
        }

        var pictures = flac is not null ? flac.Pictures : xiph.Pictures;
        tagData.Picture = pictures.Select(x => new Picture(x)).ToList();
        return tagData;
    }

    public override void Write(TagData data)
    {
        foreach (var key in xiph)
        {
            xiph.RemoveField(key);
        }

        foreach (var field in data.Fields)
        {
            xiph.SetField(field.Key, field.Text.ToArray());
        }

        var pictures = data.Picture.Select(p => new Picture(p)).ToArray<IPicture>();
        if (flac is not null)
        {
            flac.Pictures = pictures;
        }
        else
        {
            xiph.Pictures = pictures;
        }
    }
}

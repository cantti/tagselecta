using TagLib;

namespace TagSelecta.Shared.Tagging;

public static class TagDataCloner
{
    public static TagData Clone(TagData source)
    {
        var clone = new TagData { Picture = ClonePictures(source.Picture) };
        foreach (var field in source.Fields)
        {
            clone.SetValue(field.Key, field.Text);
        }
        return clone;
    }

    private static List<Picture> ClonePictures(List<Picture> source)
    {
        var list = new List<Picture>();
        foreach (var pic in source)
        {
            list.Add(ClonePicture(pic));
        }

        return list;
    }

    private static Picture ClonePicture(Picture source)
    {
        return new Picture
        {
            Type = source.Type,
            Filename = source.Filename,
            MimeType = source.MimeType,
            Description = source.Description,
            Data = [.. source.Data.Data],
        };
    }
}

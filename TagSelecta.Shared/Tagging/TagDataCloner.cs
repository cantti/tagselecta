using TagLib;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Shared.Tagging;

public static class TagDataCloner
{
    public static TagData Clone(TagData source)
    {
        var clone = new TagData { Picture = ClonePictures(source.Picture) };
        CloneExtra(clone, source.Extra);
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Extra))
        )
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(clone, prop.GetValue(source));
            }
            else if (prop.PropertyType == typeof(List<string>))
            {
                prop.SetValue(clone, CloneList((List<string>)prop.GetValue(source)!));
            }
            else
            {
                throw new TagSelectaException($"Unsupported property type: {prop.PropertyType}");
            }
        }

        return clone;
    }

    private static List<string> CloneList(List<string> source)
    {
        return [.. source];
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

    private static void CloneExtra(TagData clone, IEnumerable<ExtraField> source)
    {
        foreach (var field in source)
        {
            clone.SetExtraField(field.Key, field.Text);
        }
    }
}

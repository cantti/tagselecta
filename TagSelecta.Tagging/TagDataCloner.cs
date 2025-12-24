namespace TagSelecta.Tagging;

public static class TagDataCloner
{
    public static TagData Clone(TagData source)
    {
        var clone = new TagData
        {
            Picture = ClonePictures(source.Picture),
            Custom = CloneCustom(source.Custom),
        };
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
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
                throw new InvalidOperationException(
                    $"Unsupported property type: {prop.PropertyType}"
                );
            }
        }
        return clone;
    }

    private static List<string> CloneList(List<string> source) => [.. source];

    private static List<TagLib.Picture> ClonePictures(List<TagLib.Picture> source)
    {
        var list = new List<TagLib.Picture>();
        foreach (var pic in source)
        {
            list.Add(ClonePicture(pic));
        }
        return list;
    }

    private static TagLib.Picture ClonePicture(TagLib.Picture source)
    {
        return new TagLib.Picture
        {
            Type = source.Type,
            Filename = source.Filename,
            MimeType = source.MimeType,
            Description = source.Description,
            Data = [.. source.Data.Data],
        };
    }

    private static List<CustomField> CloneCustom(IEnumerable<CustomField> source)
    {
        var list = new List<CustomField>();
        foreach (var field in source)
        {
            list.Add(new CustomField(field.Key, field.Text));
        }
        return list;
    }
}

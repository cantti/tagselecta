namespace TagSelecta.Tagging;

public class TagDataCloner
{
    public static TagData Clone(TagData tagData)
    {
        var clone = new TagData();
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(x => x.Name != nameof(TagData.Pictures) && x.CanWrite)
        )
        {
            var val = prop.GetValue(tagData);
            if (val == null)
            {
                prop.SetValue(clone, null);
                continue;
            }
            if (val is List<string> list)
            {
                prop.SetValue(clone, new List<string>(list));
            }
            else
            {
                prop.SetValue(clone, val);
            }
        }
        clone.Pictures =
        [
            .. tagData.Pictures.Select(x => new TagLib.Picture
            {
                Data = x.Data.ToArray(),
                Description = x.Description,
                Filename = x.Filename,
                MimeType = x.MimeType,
                Type = x.Type,
            }),
        ];

        return clone;
    }
}

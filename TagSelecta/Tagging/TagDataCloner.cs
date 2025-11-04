namespace TagSelecta.Tagging;

public class TagDataCloner
{
    public static TagData Clone(TagData obj1)
    {
        var clone = new TagData();
        foreach (var prop in typeof(TagData).GetProperties())
        {
            var val = prop.GetValue(obj1);
            if (val == null)
            {
                prop.SetValue(clone, null);
                continue;
            }
            if (val is List<string> list)
            {
                prop.SetValue(clone, new List<string>(list));
            }
            else if (val is List<TagLib.IPicture> pics)
            {
                prop.SetValue(
                    clone,
                    pics.Select(x =>
                            (TagLib.IPicture)
                                new TagLib.Picture
                                {
                                    Data = x.Data.ToArray(),
                                    Description = x.Description,
                                    Filename = x.Filename,
                                    MimeType = x.MimeType,
                                    Type = x.Type,
                                }
                        )
                        .ToList()
                );
            }
            else
            {
                prop.SetValue(clone, val);
            }
        }

        return clone;
    }
}

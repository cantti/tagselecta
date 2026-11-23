namespace TagSelecta.Shared.Tagging;

public static class TagDataCloner
{
    public static TagData Clone(TagData source)
    {
        var clone = new TagData();
        foreach (var field in source.Fields)
        {
            clone.SetValue(field.Key, field.Text);
        }
        foreach (var picture in source.Picture)
        {
            clone.AddPicture(picture);
        }
        return clone;
    }
}

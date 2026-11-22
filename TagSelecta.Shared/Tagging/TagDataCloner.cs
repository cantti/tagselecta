namespace TagSelecta.Shared.Tagging;

public static class TagDataCloner
{
    public static TagData Clone(TagData source)
    {
        var clone = new TagData { Picture = [.. source.Picture] };
        foreach (var field in source.Fields)
        {
            clone.SetValue(field.Key, field.Text);
        }

        return clone;
    }
}

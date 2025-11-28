using System.Reflection;

namespace TagSelecta.Tagging;

public static class TagDataComparer
{
    public static bool TagDataEqual(TagData obj1, TagData obj2)
    {
        // compare normal tags
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p =>
                    p.GetCustomAttribute<BuiltinFieldAttribute>() != null
                    || p.Name == nameof(TagData.Custom)
                    || p.Name == nameof(TagData.Picture)
                )
        )
        {
            var val1 = prop.GetValue(obj1);
            var val2 = prop.GetValue(obj2);
            if (!PropertiesEqual(val1, val2))
            {
                return false;
            }
        }
        return true;
    }

    public static bool PicturesEqual(TagLib.Picture? p1, TagLib.Picture? p2)
    {
        if (ReferenceEquals(p1, p2))
            return true;

        if (p1 == null || p2 == null)
            return false;

        // Filename intentionally ignored

        if (p1.Description != p2.Description)
            return false;

        if (p1.MimeType != p2.MimeType)
            return false;

        if (p1.Type != p2.Type)
            return false;

        if (!(p1.Data ?? []).SequenceEqual(p2.Data ?? []))
            return false;

        return true;
    }

    private static bool PropertiesEqual(object? val1, object? val2)
    {
        if (ReferenceEquals(val1, val2))
            return true;

        if (val1 == null || val2 == null)
            return false;

        // Must match exactly
        var t = val1.GetType();
        if (t != val2.GetType())
            return false;

        // List<string>
        if (t == typeof(List<string>))
        {
            var a = (List<string>)val1;
            var b = (List<string>)val2;
            return a.SequenceEqual(b);
        }

        // List<CustomField>
        if (t == typeof(List<CustomField>))
        {
            var a = (List<CustomField>)val1;
            var b = (List<CustomField>)val2;

            if (a.Count != b.Count)
                return false;

            return a.All(kv => b.FirstOrDefault(x => x.Key == kv.Key)?.Text == kv.Text);
        }

        // List<TagLib.Picture>
        if (t == typeof(List<TagLib.Picture>))
        {
            var a = (List<TagLib.Picture>)val1;
            var b = (List<TagLib.Picture>)val2;

            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                var p1 = a[i];
                var p2 = b[i];
                if (!PicturesEqual(p1, p2))
                {
                    return false;
                }
            }

            return true;
        }

        // Default: basic .Equals
        return val1.Equals(val2);
    }
}

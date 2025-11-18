using System.Reflection;

namespace TagSelecta.Tagging;

public static class TagDataComparer
{
    public static bool AreEqual(TagData obj1, TagData obj2)
    {
        // compare normal tags
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p =>
                    p.GetCustomAttribute<TagDataFieldAttribute>() != null
                    || p.Name == nameof(TagData.Custom)
                )
        )
        {
            var val1 = prop.GetValue(obj1);
            var val2 = prop.GetValue(obj2);
            if (!FieldsEqual(val1, val2))
            {
                return false;
            }
        }
        return true;
    }

    public static bool FieldsEqual(object? val1, object? val2)
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

                if (p1.Description != p2.Description)
                    return false;

                // Filename intentionally ignored

                if (p1.MimeType != p2.MimeType)
                    return false;

                if (p1.Type != p2.Type)
                    return false;

                if (!(p1.Data ?? []).SequenceEqual(p2.Data ?? []))
                    return false;
            }

            return true;
        }

        // Default: basic .Equals
        return val1.Equals(val2);
    }
}

using System.Reflection;

namespace TagSelecta.Tagging;

public static class TagDataComparer
{
    public static bool TagDataEquals(TagData obj1, TagData obj2)
    {
        // compare normal tags
        // todo rewrite without reflection and attribute
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<PrintableAttribute>() != null)
        )
        {
            var val1 = prop.GetValue(obj1);
            var val2 = prop.GetValue(obj2);
            if (!ValueEquals(val1, val2))
            {
                return false;
            }
        }
        // compare custom
        if (
            obj1.Custom.Count != obj2.Custom.Count
            || !obj1.Custom.All(kv =>
                obj2.Custom.FirstOrDefault(x => x.Key == kv.Key)?.Value == kv.Value
            )
        )
        {
            return false;
        }
        return true;
    }

    public static bool ValueEquals(object? val1, object? val2)
    {
        if (val1 == null && val2 == null)
            return true;

        if (val1 == null || val2 == null)
            return false;

        // list of strings
        if (val1 is List<string> list1 && val2 is List<string> list2)
        {
            if (!list1.SequenceEqual(list2))
                return false;
        }
        else if (
            val1 is Dictionary<string, string> dict1
            && val2 is Dictionary<string, string> dict2
        )
        {
            if (
                dict1.Count != dict2.Count
                || !dict1.All(kv =>
                    dict2.TryGetValue(kv.Key, out var val)
                    && string.Equals(kv.Value, val, StringComparison.OrdinalIgnoreCase)
                )
            )
                return false;
        }
        // pictures
        else if (val1 is List<TagLib.Picture> pics1 && val2 is List<TagLib.Picture> pics2)
        {
            if (pics1.Count != pics2.Count)
                return false;

            for (int i = 0; i < pics1.Count; i++)
            {
                var p1 = pics1[i];
                var p2 = pics2[i];

                if (p1.Description != p2.Description)
                    return false;

                // picture file name should be ignored, because they are not stored in actual metadata
                // if (p1.Filename != p2.Filename)
                //     return false;

                if (p1.MimeType != p2.MimeType)
                    return false;

                if (p1.Type != p2.Type)
                    return false;

                // do not care about nulls here
                if (!(p1.Data ?? []).SequenceEqual(p2.Data ?? []))
                    return false;
            }
        }
        // uint, double etc
        else if (!val1.Equals(val2))
        {
            return false;
        }
        return true;
    }
}

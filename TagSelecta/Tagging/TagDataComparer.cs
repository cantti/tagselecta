namespace TagSelecta.Tagging;

public static class TagDataComparer
{
    public static bool TagDataEquals(TagData obj1, TagData obj2)
    {
        // compare everything except pictures
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(x => x.Name != nameof(TagData.Picture))
        )
        {
            var val1 = prop.GetValue(obj1);
            var val2 = prop.GetValue(obj2);

            if (val1 == null && val2 == null)
                continue;

            if (val1 == null || val2 == null)
                return false;

            if (val1 is List<string> list1 && val2 is List<string> list2)
            {
                if (!list1.SequenceEqual(list2))
                    return false;
            }
            // uint, double etc
            else if (!val1.Equals(val2))
            {
                return false;
            }
        }

        // compare pictures
        return PicturesEqual(obj1, obj2);
    }

    public static bool PicturesEqual(TagData obj1, TagData obj2)
    {
        var pics1 = obj1.Picture;
        var pics2 = obj2.Picture;

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

        return true;
    }
}

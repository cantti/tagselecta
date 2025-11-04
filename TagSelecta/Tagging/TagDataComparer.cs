namespace TagSelecta.Tagging;

public static class TagDataComparer
{
    public static bool AreEqual(TagData? obj1, TagData? obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;

        if (obj1 == null || obj2 == null)
            return false;

        foreach (var prop in typeof(TagData).GetProperties())
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
            else if (val1 is List<TagLib.Picture> pics1 && val2 is List<TagLib.Picture> pics2)
            {
                if (pics1.Count != pics2.Count)
                    return false;

                for (int i = 0; i < pics1.Count; i++)
                {
                    var p1 = pics1[i];
                    var p2 = pics2[i];

                    if (p1?.Data == null && p2?.Data == null)
                        continue;
                    if (p1?.Data == null || p2?.Data == null)
                        return false;
                    if (!p1.Data.SequenceEqual(p2.Data))
                        return false;
                }
            }
            // uint, double etc
            else if (!val1.Equals(val2))
            {
                return false;
            }
        }

        return true;
    }
}

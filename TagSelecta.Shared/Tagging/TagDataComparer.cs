namespace TagSelecta.Shared.Tagging;

public static class TagDataComparer
{
    public static bool AreEqual(TagData? a, TagData? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a == null || b == null)
            return false;
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p =>
                    p.PropertyType == typeof(string) || p.PropertyType == typeof(List<string>)
                )
        )
        {
            if (prop.PropertyType == typeof(string))
            {
                var val1 = (string)prop.GetValue(a)!;
                var val2 = (string)prop.GetValue(b)!;
                if (!Eq(val1, val2))
                {
                    return false;
                }
            }
            else if (prop.PropertyType == typeof(List<string>))
            {
                var val1 = (List<string>)prop.GetValue(a)!;
                var val2 = (List<string>)prop.GetValue(b)!;
                if (!ListEq(val1, val2))
                {
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported property type: {prop.PropertyType}"
                );
            }
        }
        return PictureListEq(a.Picture, b.Picture) && CustomListEq(a.Custom, b.Custom);
    }

    private static bool Eq(string a, string b) => a == b;

    private static bool ListEq(List<string>? a, List<string>? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;

        return a.SequenceEqual(b);
    }

    private static bool PictureListEq(List<TagLib.Picture>? a, List<TagLib.Picture>? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (!PictureEq(a[i], b[i]))
                return false;
        }

        return true;
    }

    public static bool PictureEq(TagLib.Picture? a, TagLib.Picture? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a == null || b == null)
            return false;

        return a.Type == b.Type
            && a.MimeType == b.MimeType
            && a.Description == b.Description
            && a.MimeType == b.MimeType
            && a.Data?.Data.SequenceEqual(b.Data?.Data ?? []) == true;
    }

    private static bool CustomListEq(IReadOnlyList<CustomField>? a, IReadOnlyList<CustomField>? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a == null || b == null)
            return false;
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (!(a[i] == b[i]))
                return false;
        }

        return true;
    }
}

namespace TagSelecta.Shared.Tagging;

public static class TagDataComparer
{
    public static bool AreEqual(TagData? a, TagData? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        return PictureListEq(a.Picture, b.Picture) && FieldListEq(a.Fields, b.Fields);
    }

    private static bool PictureListEq(IReadOnlyList<TagPicture>? a, IReadOnlyList<TagPicture>? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        if (a.Count != b.Count)
        {
            return false;
        }

        for (var i = 0; i < a.Count; i++)
        {
            if (!PictureEq(a[i], b[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static bool PictureEq(TagPicture? a, TagPicture? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        return a.Type == b.Type
            && a.MimeType == b.MimeType
            && a.Description == b.Description
            && a.Data.SequenceEqual(b.Data);
    }

    private static bool FieldListEq(IReadOnlyList<TagField>? a, IReadOnlyList<TagField>? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        if (a.Count != b.Count)
        {
            return false;
        }

        for (var i = 0; i < a.Count; i++)
        {
            if (a[i].Key != b[i].Key || !a[i].Text.SequenceEqual(b[i].Text))
            {
                return false;
            }
        }

        return true;
    }
}

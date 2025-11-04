using TagSelecta.Tagging;

namespace TagSelecta.Actions.Base;

public static class FieldNameValidation
{
    public static bool Validate(string field)
    {
        var tagDataProps = typeof(TagData).GetProperties();
        return tagDataProps.Any(x =>
            x.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase)
        );
    }

    public static List<string> NormalizeFields(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }
}

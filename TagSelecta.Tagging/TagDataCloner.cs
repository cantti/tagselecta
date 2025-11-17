using System.Reflection;

namespace TagSelecta.Tagging;

public class TagDataCloner
{
    public static TagData Clone(TagData source)
    {
        var clone = new TagData();

        var props = typeof(TagData)
            .GetProperties()
            .Where(p =>
                p.GetCustomAttribute<TagDataFieldAttribute>() != null
                || p.Name == nameof(TagData.Custom)
            );

        foreach (var prop in props)
        {
            var originalValue = prop.GetValue(source);
            var copiedValue = DeepCopyValue(originalValue);
            prop.SetValue(clone, copiedValue);
        }

        return clone;
    }

    private static object? DeepCopyValue(object? value)
    {
        if (value == null)
            return null;

        var type = value.GetType();

        // Primitive, string, decimal, datetime, etc.
        if (type.IsValueType || type == typeof(string))
            return value;

        // IList<T>
        if (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
            var listType = typeof(List<>).MakeGenericType(type.GetGenericArguments());

            var newListObj =
                Activator.CreateInstance(listType)
                ?? throw new InvalidOperationException(
                    $"Could not create list of type {listType}."
                );

            var newList = (System.Collections.IList)newListObj;

            foreach (var item in (System.Collections.IEnumerable)value)
                newList.Add(DeepCopyValue(item));

            return newList;
        }

        // Special case: TagLib.Picture
        if (type == typeof(TagLib.Picture))
        {
            var pic = (TagLib.Picture)value;
            return new TagLib.Picture
            {
                Data = pic.Data?.ToArray(),
                Description = pic.Description,
                Filename = pic.Filename,
                MimeType = pic.MimeType,
                Type = pic.Type,
            };
        }

        // Special case: CustomField
        if (type == typeof(CustomField))
        {
            var c = (CustomField)value;
            return new CustomField(c.Key, c.Text);
        }

        // Fallback: try shallow clone
        return value;
    }
}

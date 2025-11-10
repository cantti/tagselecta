using System.Reflection;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Actions.TagDataActions;

public static class TagDataActionHelper
{
    public static bool ValidateFieldNameList(IAnsiConsole console, IEnumerable<string> fields)
    {
        var tagDataProps = typeof(TagData)
            .GetProperties()
            .Where(x => x.GetCustomAttribute<EditableAttribute>() != null);
        foreach (var field in fields)
        {
            if (
                !tagDataProps.Any(x =>
                    x.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase)
                )
            )
            {
                console.MarkupLineInterpolated($"[red]Unknown field: {field}[/]");
                return false;
            }
        }
        return true;
    }

    public static List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }
}

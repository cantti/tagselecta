using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Actions.Base;

public static class ActionHelper
{
    public static List<string> NormalizeFields(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }

    public static bool ValidateFieldName(string field)
    {
        var tagDataProps = typeof(TagData).GetProperties();
        return tagDataProps.Any(x =>
            x.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase)
        );
    }

    public static bool ValidateFieldNameList(IEnumerable<string> fields, IAnsiConsole console)
    {
        foreach (var tagToKeep in fields)
        {
            if (!ValidateFieldName(tagToKeep))
            {
                console.MarkupLineInterpolated($"[red]Unknown tag: {tagToKeep}[/]");
                return false;
            }
        }
        return true;
    }
}

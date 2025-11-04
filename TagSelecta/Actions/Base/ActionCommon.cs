using Spectre.Console;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions.Base;

public class ActionCommon(IAnsiConsole console)
{
    public bool TagDataChanged(TagData tagData1, TagData tagData2)
    {
        if (TagDataComparer.AreEqual(tagData1, tagData2))
        {
            console.MarkupLine("Nothing to change.");
            return false;
        }
        else
        {
            Printer.PrintComparison(console, tagData1, tagData2);
            return true;
        }
    }

    public bool ValidateFieldNameList(IEnumerable<string> fields)
    {
        foreach (var tagToKeep in fields)
        {
            if (!FieldNameValidation.Validate(tagToKeep))
            {
                console.MarkupLineInterpolated($"[red]Unknown tag: {tagToKeep}[/]");
                return false;
            }
        }
        return true;
    }
}

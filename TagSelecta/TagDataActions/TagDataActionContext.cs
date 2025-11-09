using System.Reflection;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataActions;

public class TagDataActionContext<TSettings>(IAnsiConsole console)
{
    public required List<string> Files { get; set; }

    public required TSettings Settings { get; set; }

    private string? _currentFile;
    public string CurrentFile => _currentFile ?? throw new InvalidOperationException();

    private int? _currentFileIndex;
    public int CurrentFileIndex => _currentFileIndex ?? throw new InvalidOperationException();

    private TagData? _tagData;
    public TagData TagData => _tagData ?? throw new InvalidOperationException();

    public void SetCurrentFile(string currentFile, int currentFileIndex, TagData? tagData)
    {
        _tagData = tagData;
        _currentFile = currentFile;
        _currentFileIndex = currentFileIndex;
    }

    public bool ValidateFieldNameList(IEnumerable<string> fields)
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

    public List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }
}

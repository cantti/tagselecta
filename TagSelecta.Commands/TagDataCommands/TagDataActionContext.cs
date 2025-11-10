using TagSelecta.Tagging;

namespace TagSelecta.Commands.TagDataCommands;

public class TagDataActionContext<TSettings>
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
}

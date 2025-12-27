using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Common;

public class TagDataOperation
{
    public TagDataOperation(string path, TagData tagData)
    {
        Path = path;
        TagData = tagData;
        OriginalTagData = tagData.Clone();
    }

    public string Path { get; private set; }
    public TagData TagData { get; private set; }
    public TagData OriginalTagData { get; private set; }
    public bool IsSaved { get; private set; }
    public Exception Exception { get; private set; }

    public bool HasChanges => !TagDataComparer.AreEqual(TagData, OriginalTagData);

    public void MarkSaved()
    {
        IsSaved = true;
        OriginalTagData = TagData.Clone();
    }

    public void MarkError(Exception ex)
    {
        IsSaved = true;
        Exception = ex;
    }
}

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public class TagDataOperation
{
    public TagDataOperation(string path, Tagging.TagData tagData)
    {
        Path = path;
        TagData = tagData;
        OriginalTagData = tagData.Clone();
    }

    public string Path { get; private set; }
    public Tagging.TagData TagData { get; private set; }
    public Tagging.TagData OriginalTagData { get; private set; }
    public bool IsSaved { get; private set; }
    public Exception? Exception { get; private set; }
    public bool HasChanges { get; set; }

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

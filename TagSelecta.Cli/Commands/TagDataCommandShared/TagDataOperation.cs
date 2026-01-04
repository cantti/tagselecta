using TagSelecta.Tagging;

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
    public Tagging.TagData TagData { get; }
    public Tagging.TagData OriginalTagData { get; private set; }
    public TagDataOperationStatus Status { get; private set; } = TagDataOperationStatus.Pending;
    public Exception? Exception { get; private set; }
    public bool HasChanges { get; private set; }

    public void CheckForChanges()
    {
        HasChanges = !TagDataComparer.AreEqual(TagData, OriginalTagData);
    }

    public void Write(ITagger tagger)
    {
        try
        {
            tagger.WriteTags(Path, TagData);
            Status = TagDataOperationStatus.Written;
            OriginalTagData = TagData.Clone();
            HasChanges = false;
        }
        catch (Exception ex)
        {
            MarkError(ex);
        }
    }

    public void MarkError(Exception ex)
    {
        Status = TagDataOperationStatus.Failed;
        Exception = ex;
    }
}

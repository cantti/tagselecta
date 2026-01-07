using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tui;

public class TagDataOperation : IFileContext
{
    public TagDataOperation(string currentPath, Tagging.TagData currentTagData)
    {
        CurrentPath = currentPath;
        CurrentTagData = currentTagData;
        OriginalTagData = currentTagData.Clone();
        OriginalPath = currentPath;
    }

    public string CurrentPath { get; set; }
    public string OriginalPath { get; private set; }
    public Tagging.TagData CurrentTagData { get; private set; }
    public Tagging.TagData OriginalTagData { get; private set; }
    public Exception? Exception { get; private set; }
    public bool HasChanges { get; private set; }
    public bool IsSelected { get; set; }

    public void Undo()
    {
        CurrentTagData = OriginalTagData.Clone();
        HasChanges = false;
        Exception = null;
    }

    public void CheckForChanges()
    {
        HasChanges =
            !TagDataComparer.AreEqual(CurrentTagData, OriginalTagData)
            || CurrentPath != OriginalPath;
    }

    public void Write(ITagger tagger, IFileSystem fs)
    {
        try
        {
            if (!TagDataComparer.AreEqual(CurrentTagData, OriginalTagData))
            {
                tagger.WriteTags(OriginalPath, CurrentTagData);
                OriginalTagData = CurrentTagData.Clone();
            }
            if (CurrentPath != OriginalPath)
            {
                fs.Move(OriginalPath, CurrentPath);
                OriginalPath = CurrentPath;
            }
            HasChanges = false;
        }
        catch (Exception ex)
        {
            MarkError(ex);
        }
    }

    public void MarkError(Exception ex)
    {
        Exception = ex;
    }
}

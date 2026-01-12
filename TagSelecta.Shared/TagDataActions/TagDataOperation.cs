using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public class TagDataOperation : IFileContext
{
    private TagData _originalTagData;

    public TagDataOperation(string currentPath, TagData currentTagData)
    {
        CurrentPath = currentPath;
        CurrentTagData = currentTagData;
        _originalTagData = currentTagData.Clone();
        OriginalPath = currentPath;
    }

    public string CurrentPath { get; set; }
    public string OriginalPath { get; private set; }
    public TagData CurrentTagData { get; private set; }

    // expose TagData as read-only
    public TagData OriginalTagData => _originalTagData.Clone();
    public Exception? Exception { get; private set; }
    public bool HasChanges { get; private set; }
    public bool IsSelected { get; set; }

    public void Undo()
    {
        CurrentTagData = _originalTagData.Clone();
        HasChanges = false;
        Exception = null;
    }

    public void CheckForChanges()
    {
        HasChanges =
            !TagDataComparer.AreEqual(CurrentTagData, _originalTagData)
            || CurrentPath != OriginalPath;
    }

    public void Write(ITagger tagger, IFileSystem fs)
    {
        try
        {
            if (!TagDataComparer.AreEqual(CurrentTagData, _originalTagData))
            {
                tagger.WriteTags(OriginalPath, CurrentTagData);
                _originalTagData = CurrentTagData.Clone();
            }
            if (CurrentPath != OriginalPath)
            {
                if (!fs.Exists(CurrentPath))
                {
                    fs.CreateDirectory(Path.GetDirectoryName(CurrentPath)!);
                    fs.Move(OriginalPath, CurrentPath);
                    OriginalPath = CurrentPath;
                }
                else
                {
                    //todo throw exception
                }
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

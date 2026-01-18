using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public class TagDataOperation : ITagDataActionContext
{
    private TagData _backupTagData = null!;

    public TagDataOperation(string currentPath, TagData currentTagData)
    {
        CurrentTagData = currentTagData;
        SetCurrentPath(currentPath, MoveOptions.None);
        UpdateBackup();
    }

    public string CurrentPath { get; private set; } = null!;
    public string BackupPath { get; private set; } = null!;
    public TagData CurrentTagData { get; private set; }

    // expose TagData as read-only
    public TagData BackupTagData => _backupTagData.Clone();
    public Exception? Exception { get; private set; }
    public bool HasChanges { get; private set; }
    public bool IsSelected { get; set; }

    public MoveOptions MoveOptions { get; private set; }

    public void SetCurrentPath(string path, MoveOptions moveOptions)
    {
        CurrentPath = path;
        MoveOptions = moveOptions;
    }

    public void Undo()
    {
        CurrentTagData = _backupTagData.Clone();
        SetCurrentPath(BackupPath, MoveOptions.None);
        HasChanges = false;
        Exception = null;
    }

    public void CheckForChanges()
    {
        HasChanges =
            !TagDataComparer.AreEqual(CurrentTagData, _backupTagData) || CurrentPath != BackupPath;
    }

    public void UpdateBackup()
    {
        _backupTagData = CurrentTagData.Clone();
        BackupPath = CurrentPath;
        MoveOptions = MoveOptions.None;
        HasChanges = false;
    }

    public void MarkError(Exception ex)
    {
        Exception = ex;
    }

    public void ResetError()
    {
        Exception = null;
    }
}

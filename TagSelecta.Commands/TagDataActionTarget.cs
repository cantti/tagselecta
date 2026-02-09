using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands;

public class TagDataActionTarget : ITagDataActionTarget
{
    private TagData _backupTagData = null!;
    private TagData _currentTagData;

    public TagDataActionTarget(string currentPath, TagData currentTagData)
    {
        _currentTagData = currentTagData;
        UpdatePath(currentPath, MoveOptions.None);
        UpdateBackup();
    }

    public Exception? Exception { get; private set; }

    public bool HasChanges { get; private set; }

    public bool IsSelected { get; set; }

    public string CurrentPath { get; private set; } = null!;

    public string BackupPath { get; private set; } = null!;

    public TagData CurrentTagData => _currentTagData.Clone();

    public TagData BackupTagData => _backupTagData.Clone();

    public MoveOptions MoveOptions { get; private set; }

    public void UpdatePath(string path, MoveOptions moveOptions)
    {
        CurrentPath = path;
        MoveOptions = moveOptions;
        CheckForChanges();
    }

    public void UpdateTagData(TagData tagData)
    {
        _currentTagData = tagData.Clone();
        CheckForChanges();
    }

    public void Undo()
    {
        _currentTagData = _backupTagData.Clone();
        UpdatePath(BackupPath, MoveOptions.None);
        HasChanges = false;
        Exception = null;
    }

    public void UpdateBackup()
    {
        _backupTagData = _currentTagData.Clone();
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

    private void CheckForChanges()
    {
        HasChanges =
            !TagDataComparer.AreEqual(_currentTagData, _backupTagData) || CurrentPath != BackupPath;
    }
}

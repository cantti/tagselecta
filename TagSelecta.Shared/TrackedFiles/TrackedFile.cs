using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TrackedFiles;

public class TrackedFile : ITagDataActionTarget
{
    private TagData _backupTagData = null!;
    private TagData _currentTagData;
    private string _currentPath = null!;
    private string _backupPath = null!;
    private MoveOptions _moveOptions;

    public Exception? Exception { get; private set; }

    public bool HasChanges { get; private set; }

    public bool IsSelected { get; set; }

    public TrackedFile(string currentPath, TagData currentTagData)
    {
        _currentTagData = currentTagData;
        SetCurrentPath(currentPath, MoveOptions.None);
        UpdateBackup();
    }

    public string GetCurrentPath()
    {
        return _currentPath;
    }

    public string GetBackupPath()
    {
        return _backupPath;
    }

    public TagData GetCurrentTagData()
    {
        return _currentTagData.Clone();
    }

    public TagData GetBackupTagData()
    {
        return _backupTagData.Clone();
    }

    public MoveOptions GetMoveOptions()
    {
        return _moveOptions;
    }

    public void SetCurrentPath(string path, MoveOptions moveOptions)
    {
        _currentPath = path;
        _moveOptions = moveOptions;
        CheckForChanges();
    }

    public void SetCurrentTagData(TagData tagData)
    {
        _currentTagData = tagData.Clone();
        CheckForChanges();
    }

    public void Undo()
    {
        _currentTagData = _backupTagData.Clone();
        SetCurrentPath(_backupPath, MoveOptions.None);
        HasChanges = false;
        Exception = null;
    }

    internal void UpdateBackup()
    {
        _backupTagData = _currentTagData.Clone();
        _backupPath = _currentPath;
        _moveOptions = MoveOptions.None;
        HasChanges = false;
    }

    internal void MarkError(Exception ex)
    {
        Exception = ex;
    }

    internal void ResetError()
    {
        Exception = null;
    }

    private void CheckForChanges()
    {
        HasChanges =
            !TagDataComparer.AreEqual(_currentTagData, _backupTagData)
            || _currentPath != _backupPath;
    }
}

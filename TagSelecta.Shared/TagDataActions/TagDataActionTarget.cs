using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public class TagDataActionTarget : ITagDataActionTarget
{
    private TagData _backupTagData = null!;
    private TagData _currentTagData;
    private string _currentPath = null!;
    private string _backupPath = null!;
    private MoveOptions _moveOptions;

    public Exception? Exception { get; private set; }

    public bool HasChanges { get; private set; }

    public bool IsSelected { get; set; }

    public TagDataActionTarget(string currentPath, TagData currentTagData)
    {
        _currentTagData = currentTagData;
        UpdatePath(currentPath, MoveOptions.None);
        UpdateBackup();
    }

    public string CurrentPath => _currentPath;

    public string BackupPath => _backupPath;

    public TagData CurrentTagData => _currentTagData.Clone();

    public TagData BackupTagData => _backupTagData.Clone();

    public void UpdatePath(string path, MoveOptions moveOptions)
    {
        _currentPath = path;
        _moveOptions = moveOptions;
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
        UpdatePath(_backupPath, MoveOptions.None);
        HasChanges = false;
        Exception = null;
    }

    public void Write(ITagger tagger, IFileSystem fs)
    {
        ResetError();
        try
        {
            if (!TagDataComparer.AreEqual(CurrentTagData, BackupTagData))
            {
                tagger.WriteTags(_backupPath, CurrentTagData);
            }

            if (_currentPath != _backupPath && !fs.Exists(_currentPath))
            {
                var destDir = Path.GetDirectoryName(_currentPath)!;

                // create a directory with subdirectories
                fs.CreateDirectory(destDir);

                // move audio file
                fs.Move(_backupPath, _currentPath);

                // move other files
                if (!_moveOptions.HasFlag(MoveOptions.DoNotMoveOtherFiles))
                {
                    var otherFiles = fs.GetFiles(Path.GetDirectoryName(_backupPath)!)
                        .Where(f =>
                            !AudioFileScanner.AllowedExtensions.Contains(
                                Path.GetExtension(f).ToLower()
                            )
                        );
                    foreach (var file in otherFiles)
                    {
                        var dest = Path.Combine(destDir, Path.GetFileName(file));
                        if (!fs.Exists(dest))
                        {
                            fs.Move(file, dest);
                        }
                    }
                }
                // delete empty directories
                if (
                    !_moveOptions.HasFlag(MoveOptions.KeepEmptyDirectories)
                    && fs.IsDirectoryEmpty(Path.GetDirectoryName(_backupPath)!)
                )
                {
                    fs.DeleteDirectory(Path.GetDirectoryName(_backupPath)!);
                }
            }
            UpdateBackup();
        }
        catch (Exception ex)
        {
            MarkError(ex);
        }
    }

    public async Task ExecuteTagDataAction(
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    )
    {
        ResetError();
        try
        {
            await action.Execute(context, token);
        }
        catch (Exception ex)
        {
            MarkError(ex);
        }
    }

    private void UpdateBackup()
    {
        _backupTagData = _currentTagData.Clone();
        _backupPath = _currentPath;
        _moveOptions = MoveOptions.None;
        HasChanges = false;
    }

    private void MarkError(Exception ex)
    {
        Exception = ex;
    }

    private void ResetError()
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

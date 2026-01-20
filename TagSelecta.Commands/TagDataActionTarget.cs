using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands;

public class TagDataActionTarget : ITagDataActionTarget
{
    private readonly ITagger _tagger;
    private readonly IFileSystem _fs;
    private TagData _backupTagData = null!;
    private TagData _currentTagData;
    private string _currentPath = null!;
    private string _path = null!;
    private MoveOptions _moveOptions;

    public Exception? Exception { get; private set; }

    public bool HasChanges { get; private set; }

    public bool IsSelected { get; set; }

    public TagDataActionTarget(
        ITagger tagger,
        IFileSystem fs,
        string currentPath,
        TagData currentTagData
    )
    {
        _tagger = tagger;
        _fs = fs;
        _currentTagData = currentTagData;
        UpdatePath(currentPath, MoveOptions.None);
        UpdateBackup();
    }

    public string CurrentPath => _currentPath;

    public string BackupPath => _path;

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
        UpdatePath(_path, MoveOptions.None);
        HasChanges = false;
        Exception = null;
    }

    public void Write()
    {
        ResetError();
        try
        {
            if (!TagDataComparer.AreEqual(CurrentTagData, BackupTagData))
            {
                _tagger.WriteTags(_path, CurrentTagData);
            }

            if (_currentPath != _path && !_fs.Exists(_currentPath))
            {
                var destDir = Path.GetDirectoryName(_currentPath)!;

                // create a directory with subdirectories
                _fs.CreateDirectory(destDir);

                // move audio file
                _fs.Move(_path, _currentPath);

                // move other files
                if (!_moveOptions.HasFlag(MoveOptions.DoNotMoveOtherFiles))
                {
                    var otherFiles = _fs.GetFiles(System.IO.Path.GetDirectoryName(_path)!)
                        .Where(f =>
                            !AudioFileScanner.AllowedExtensions.Contains(
                                System.IO.Path.GetExtension(f).ToLower()
                            )
                        );
                    foreach (var file in otherFiles)
                    {
                        var dest = System.IO.Path.Combine(
                            destDir,
                            System.IO.Path.GetFileName(file)
                        );
                        if (!_fs.Exists(dest))
                        {
                            _fs.Move(file, dest);
                        }
                    }
                }
                // delete empty directories
                if (
                    !_moveOptions.HasFlag(MoveOptions.KeepEmptyDirectories)
                    && _fs.IsDirectoryEmpty(System.IO.Path.GetDirectoryName(_path)!)
                )
                {
                    _fs.DeleteDirectory(System.IO.Path.GetDirectoryName(_path)!);
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
        _path = _currentPath;
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
            !TagDataComparer.AreEqual(_currentTagData, _backupTagData) || _currentPath != _path;
    }
}

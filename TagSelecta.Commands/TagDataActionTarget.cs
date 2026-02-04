using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands;

public class TagDataActionTarget : ITagDataActionTarget
{
    private readonly IFileSystem _fs;
    private readonly ITagger _tagger;
    private TagData _backupTagData = null!;
    private TagData _currentTagData;
    private MoveOptions _moveOptions;

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

    public Exception? Exception { get; private set; }

    public bool HasChanges { get; private set; }

    public bool IsSelected { get; set; }

    public string CurrentPath { get; private set; } = null!;

    public string BackupPath { get; private set; } = null!;

    public TagData CurrentTagData => _currentTagData.Clone();

    public TagData BackupTagData => _backupTagData.Clone();

    public void UpdatePath(string path, MoveOptions moveOptions)
    {
        CurrentPath = path;
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
        UpdatePath(BackupPath, MoveOptions.None);
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
                _tagger.WriteTags(BackupPath, CurrentTagData);
            }

            if (CurrentPath != BackupPath && !_fs.Exists(CurrentPath))
            {
                var destDir = Path.GetDirectoryName(CurrentPath)!;

                // create a directory with subdirectories
                _fs.CreateDirectory(destDir);

                // move audio file
                _fs.Move(BackupPath, CurrentPath);

                // move other files
                if (!_moveOptions.HasFlag(MoveOptions.DoNotMoveOtherFiles))
                {
                    var otherFiles = _fs.GetFiles(Path.GetDirectoryName(BackupPath)!)
                        .Where(f =>
                            !AudioFileScanner.AllowedExtensions.Contains(
                                Path.GetExtension(f).ToLower()
                            )
                        );
                    foreach (var file in otherFiles)
                    {
                        var dest = Path.Combine(destDir, Path.GetFileName(file));
                        if (!_fs.Exists(dest))
                        {
                            _fs.Move(file, dest);
                        }
                    }
                }

                // delete empty directories
                if (
                    !_moveOptions.HasFlag(MoveOptions.KeepEmptyDirectories)
                    && _fs.IsDirectoryEmpty(Path.GetDirectoryName(BackupPath)!)
                )
                {
                    _fs.DeleteDirectory(Path.GetDirectoryName(BackupPath)!);
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
        BackupPath = CurrentPath;
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
            !TagDataComparer.AreEqual(_currentTagData, _backupTagData) || CurrentPath != BackupPath;
    }
}

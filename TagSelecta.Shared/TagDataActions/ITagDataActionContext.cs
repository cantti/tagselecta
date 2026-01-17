using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionContext
{
    public string CurrentPath { get; }
    string BackupPath { get; }
    public TagData CurrentTagData { get; }
    TagData BackupTagData { get; }
    void SetCurrentPath(string path, MoveOptions moveOptions);
}

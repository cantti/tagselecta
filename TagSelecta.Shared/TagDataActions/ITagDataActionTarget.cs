using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionTarget
{
    string GetCurrentPath();
    string GetBackupPath();
    TagData GetCurrentTagData();
    TagData GetBackupTagData();
    MoveOptions GetMoveOptions();
    void SetCurrentPath(string path, MoveOptions moveOptions);
    void SetCurrentTagData(TagData tagData);
}

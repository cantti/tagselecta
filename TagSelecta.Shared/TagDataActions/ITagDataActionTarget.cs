using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionTarget
{
    string GetCurrentPath();
    TagData GetCurrentTagData();
    void SetCurrentPath(string path, MoveOptions moveOptions);
    void SetCurrentTagData(TagData tagData);
    string GetBackupPath();
    TagData GetBackupTagData();
}

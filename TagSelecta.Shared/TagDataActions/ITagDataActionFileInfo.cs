using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionFileInfo
{
    string GetBackupPath();
    TagData GetBackupTagData();
}

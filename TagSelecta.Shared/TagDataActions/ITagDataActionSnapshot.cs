using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionSnapshot
{
    string GetBackupPath();
    TagData GetBackupTagData();
}

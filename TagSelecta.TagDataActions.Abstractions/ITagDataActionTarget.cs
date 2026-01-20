using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.Abstractions;

public interface ITagDataActionTarget
{
    TagData CurrentTagData { get; }
    TagData BackupTagData { get; }
    string BackupPath { get; }
    void UpdatePath(string path, MoveOptions moveOptions);
    void UpdateTagData(TagData tagData);
}

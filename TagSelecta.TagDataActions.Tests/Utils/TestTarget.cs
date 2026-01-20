using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.Tests.Utils;

public class TestTarget : ITagDataActionTarget
{
    public TestTarget(string path, TagData tagData)
    {
        CurrentTagData = tagData;
        BackupTagData = tagData;
        BackupPath = path;
        CurrentPath = path;
    }

    public TagData CurrentTagData { get; private set; }
    public TagData BackupTagData { get; }

    public string CurrentPath { get; private set; }
    public string BackupPath { get; }

    public void UpdatePath(string path, MoveOptions moveOptions)
    {
        CurrentPath = path;
    }

    public void UpdateTagData(TagData tagData)
    {
        CurrentTagData = tagData;
    }
}

using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

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

    public MoveOptions MoveOptions { get; set; }

    public void UpdatePath(string path, MoveOptions moveOptions)
    {
        CurrentPath = path;
        MoveOptions = moveOptions;
    }

    public void UpdateTagData(TagData tagData)
    {
        CurrentTagData = tagData;
    }
}

namespace TagSelecta.TagDataActions.Abstractions;

[Flags]
public enum MoveOptions
{
    None = 0,
    KeepEmptyDirectories = 1,
    DoNotMoveOtherFiles = 2,
}

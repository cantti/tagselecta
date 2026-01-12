using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface IFileContext
{
    public string CurrentPath { get; set; }
    string OriginalPath { get; }
    public TagData CurrentTagData { get; }
    TagData OriginalTagData { get; }
}

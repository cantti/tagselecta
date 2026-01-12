using TagSelecta.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface IFileContext
{
    public string CurrentPath { get; set; }
    public TagData CurrentTagData { get; }
    string OriginalPath { get; }
    TagData OriginalTagData { get; }
}

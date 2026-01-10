using TagSelecta.Tagging;

namespace TagSelecta.App;

public interface IFileContext
{
    public string CurrentPath { get; set; }
    public TagData CurrentTagData { get; }
}

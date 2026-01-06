using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public interface IFileContext
{
    public string CurrentPath { get; set; }
    public TagData CurrentTagData { get; }
}

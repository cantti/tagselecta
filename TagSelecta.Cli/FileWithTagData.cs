using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public class FileWithTagData
{
    public FileWithTagData(string path, TagData tagData)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        TagData = tagData ?? throw new ArgumentNullException(nameof(tagData));
    }

    public string Path { get; }
    public TagData TagData { get; }
}

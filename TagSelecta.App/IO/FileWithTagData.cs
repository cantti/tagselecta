using TagSelecta.Tagging;

namespace TagSelecta.App.IO;

public class FileWithTagData
{
    public FileWithTagData(string path, TagData tagData)
    {
        Path = path;
        TagData = tagData;
    }

    public string Path { get; set; }
    public TagData TagData { get; }
}

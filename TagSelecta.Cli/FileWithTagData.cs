using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public class FileWithTagData
{
    public required string Path { get; set; }
    public required TagData TagData { get; set; }
}

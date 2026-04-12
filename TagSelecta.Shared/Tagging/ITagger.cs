namespace TagSelecta.Shared.Tagging;

public interface ITagger
{
    TagData ReadTags(string file);
    void WriteTags(string file, TagData data);
}

namespace TagSelecta.Tagging;

public interface ITagger
{
    TagData ReadTags(string file);
    void RemoveTags(string file);
    void WriteTags(string file, TagData data);
}

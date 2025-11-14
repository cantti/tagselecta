namespace TagSelecta.Tagging;

public abstract class TagDataProcessor
{
    public abstract TagData Read();

    public abstract void Write(TagData data);
}

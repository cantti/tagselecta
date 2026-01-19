namespace TagSelecta.Shared.TagDataActions;

public class TagDataActionFileInfo(string path) : ITagDataActionFileInfo
{
    public string Path => path;
}
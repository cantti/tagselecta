using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public class TagDataActionTargetFactory(ITagger tagger, IFileSystem fs)
    : ITagDataActionTargetFactory
{
    public TagDataActionTarget Create(string path, TagData tagData)
    {
        return new TagDataActionTarget(tagger, fs, path, tagData);
    }
}

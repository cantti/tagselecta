using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionTargetFactory
{
    TagDataActionTarget Create(string path, TagData tagData);
}

using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands;

public interface ITagDataActionTargetFactory
{
    TagDataActionTarget Create(string path, TagData tagData);
}

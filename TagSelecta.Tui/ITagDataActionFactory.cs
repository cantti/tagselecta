using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Tui;

public interface ITagDataActionFactory
{
    ITagDataAction? Create(string name);
}

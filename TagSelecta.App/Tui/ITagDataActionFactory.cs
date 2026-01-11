using TagSelecta.App.Shared;

namespace TagSelecta.App.Tui;

public interface ITagDataActionFactory
{
    ITagDataAction? Create(string name);
}

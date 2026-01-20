using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui;

public interface ITagDataActionFactory
{
    ITagDataAction? Create(string name);
}

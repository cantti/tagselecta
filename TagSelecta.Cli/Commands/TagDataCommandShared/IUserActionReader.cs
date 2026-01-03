using Spectre.Console;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public interface IUserActionReader
{
    UserAction Read();
    LayoutElement RenderNavigation(bool filter);
}

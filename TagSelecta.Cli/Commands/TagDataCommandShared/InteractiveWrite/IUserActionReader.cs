namespace TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;

public interface IUserActionReader
{
    UserAction Read();
    LayoutElement RenderNavigation();
}

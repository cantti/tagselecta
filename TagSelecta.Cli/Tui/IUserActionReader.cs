using System.Text;

namespace TagSelecta.Cli.Tui;

public interface IUserActionReader
{
    ActionRequest? Read(ConsoleKeyInfo key);
    StringBuilder Buffer { get; }
    int Cursor { get; }
    InputMode Mode { get; }
}

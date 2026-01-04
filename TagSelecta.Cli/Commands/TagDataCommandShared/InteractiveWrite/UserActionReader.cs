using Spectre.Console;

namespace TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;

public class UserActionReader(IAnsiConsole console) : IUserActionReader
{
    public UserAction Read()
    {
        while (true)
        {
            var key = console.Input.ReadKey(true)?.KeyChar;
            UserAction? input = key switch
            {
                'j' => UserAction.Next,
                '\x0E' => UserAction.Next, // Ctrl+N
                'k' => UserAction.Previous,
                '\x10' => UserAction.Previous, // Ctrl+P
                'w' => UserAction.Write,
                'a' => UserAction.WriteAll,
                'f' => UserAction.ToggleFilter,
                't' => UserAction.ToggleTree,
                'q' => UserAction.Quit,
                _ => null,
            };

            if (input.HasValue)
            {
                return input.Value;
            }
        }
    }

    public LayoutElement RenderNavigation()
    {
        var keymap = new Dictionary<string, string>
        {
            { "j", "next" },
            { "k", "previous" },
            { "w", "write" },
            { "a", "write all" },
            { "f", "toggle changes filter" },
            { "t", "toggle tree" },
            { "q", "quit" },
        };
        var columns = new Columns(keymap.Select(x => new Markup($"[green]{x.Key}[/]={x.Value}")))
        {
            Expand = false,
        };
        return new LayoutElement(
            new Rows(new Text("Navigation: ", new Style(Color.Yellow)), columns),
            3
        );
    }
}

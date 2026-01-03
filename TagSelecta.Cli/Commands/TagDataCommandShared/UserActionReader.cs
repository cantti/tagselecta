using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public class UserActionReader(IAnsiConsole console) : IUserActionReader
{
    public UserAction Read()
    {
        console.Cursor.Hide();

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
                'q' => UserAction.Quit,
                _ => null,
            };

            if (input.HasValue)
            {
                console.Cursor.Show();
                return input.Value;
            }
        }
    }

    public LayoutElement RenderNavigation(bool filter)
    {
        var keymap = new Dictionary<string, string>
        {
            { "j", "next" },
            { "k", "previous" },
            { "w", "write" },
            { "a", "write all" },
            { "f", "toggle changes filter" },
            { "q", "quit" },
        };
        var columns = new Columns(keymap.Select(x => new Markup($"[green]{x.Key}[/]={x.Value}")))
        {
            Expand = false,
        };
        return new LayoutElement(
            new Rows(
                new Text("Navigation: ", new Style(Color.Yellow)),
                columns,
                new Markup($"Filter: [green]{(filter ? "on" : "off")}[/]")
            ),
            4
        );
    }
}

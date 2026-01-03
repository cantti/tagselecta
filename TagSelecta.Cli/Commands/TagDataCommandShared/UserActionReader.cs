using Spectre.Console;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public class UserActionReader(IAnsiConsole console) : IUserActionReader
{
    public UserAction Read()
    {
        console.Cursor.Hide();

        var parts = new List<string>
        {
            "[bold yellow]j[/]=next",
            "[bold yellow]k[/]=previous",
            "[bold yellow]w[/]=write",
            "[bold yellow]a[/]=write all",
            "[bold yellow]f[/]=toggle changes filter",
            "[bold yellow]q[/]=quit",
        };

        // single line
        console.MarkupLine(string.Join("[grey], [/]", parts));

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
}

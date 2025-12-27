using System.Text.RegularExpressions;
using Spectre.Console;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public static class CommandHelper
{
    public static void PrintStatusSuccess(IAnsiConsole console)
    {
        console.MarkupLine("[blue]Status[/]: [green]success[/]");
    }

    public static void PrintStatusError(IAnsiConsole console)
    {
        console.MarkupLineInterpolated($"[blue]Status[/]: [red]error[/]");
    }

    public static void PrintCurrentFile(IAnsiConsole console, string file, int index, int total)
    {
        // todo: make it configurable to print relative path
        file = Path.GetRelativePath(Environment.CurrentDirectory, file);
        console.MarkupInterpolated($"[dim]>[/] [yellow]({index + 1}/{total})[/] \"");
        var path = new TextPath(file)
            .RootColor(Color.White)
            .SeparatorColor(Color.White)
            .StemColor(Color.White)
            .LeafColor(Color.Yellow);
        console.Write(path);
        console.Write("\"");
        console.WriteLine();
    }

    public static List<string> Scan(
        IAnsiConsole console,
        IAudioFileScanner audioFileScanner,
        IEnumerable<string> path
    )
    {
        console.MarkupLine("Searching for files...");
        console.WriteLine();
        return audioFileScanner.Scan(path, true);
    }

    public static List<FileWithTagData> ScanAndRead(
        IAnsiConsole console,
        IAudioFileScanner audioFileScanner,
        ITagger tagger,
        IEnumerable<string> path
    )
    {
        var files = Scan(console, audioFileScanner, path);
        var result = new List<FileWithTagData>();
        foreach (var file in files)
        {
            var tagData = tagger.ReadTags(file);
            result.Add(new() { Path = file, TagData = tagData });
        }
        return result;
    }

    public static UserInput ReadNavigationCommand(IAnsiConsole console, bool showWrite)
    {
        console.Cursor.Hide();

        console.WriteLine(
            $"j = next, k = previous{(showWrite ? ", w = write, a = write all" : "")}, q = quit"
        );

        while (true)
        {
            var key = console.Input.ReadKey(true)?.KeyChar;
            UserInput? input = key switch
            {
                'j' => UserInput.Next,
                'k' => UserInput.Previous,
                'w' => UserInput.Write,
                'a' => UserInput.WriteAll,
                'q' => UserInput.Quit,
                _ => null,
            };
            if (input.HasValue)
            {
                console.Cursor.Show();
                return input.Value;
            }
        }
    }

    public static int ClampIndex(int index, int count)
    {
        if (index < 0)
            return 0;

        if (index >= count)
            return count - 1;

        return index;
    }

    public static string CleanFileName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input
            .Replace(Path.DirectorySeparatorChar.ToString(), "")
            .Replace(Path.AltDirectorySeparatorChar.ToString(), "");
        input = Regex.Replace(input, @"\s+", " ");
        return input;
    }
}

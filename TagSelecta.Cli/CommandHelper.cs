using System.Text.RegularExpressions;
using Spectre.Console;

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

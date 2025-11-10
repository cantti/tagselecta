using Spectre.Console;
using TagSelecta.Cli.IO;

namespace TagSelecta.Cli.Commands;

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

    public static void PrintStatusSkipped(IAnsiConsole console)
    {
        console.MarkupLineInterpolated($"[blue]Status[/]: skipped");
    }

    public static void PrintCurrentFile(IAnsiConsole console, string file, int index, int total)
    {
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

    public static List<string> GetFiles(IAnsiConsole console, IEnumerable<string> path)
    {
        console.MarkupLine("Searching for files...");

        console.WriteLine();

        var files = FileHelper.GetAllAudioFiles(path, true);

        console.MarkupLineInterpolated(
            $"[yellow]{files.Count}[/] {(files.Count == 1 ? "file" : "files")} found."
        );

        console.WriteLine();

        return files;
    }
}

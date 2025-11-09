using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.FileActions;

namespace TagSelecta.Commands;

public class FileCommand<TAction, TSettings>(TAction action, IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : BaseSettings
    where TAction : IFileAction<TSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        console.MarkupLine("Searching for files...");

        console.WriteLine();

        var files = FileHelper.GetAllAudioFiles(settings.Path, true);

        console.MarkupLineInterpolated(
            $"[yellow]{files.Count}[/] {(files.Count == 1 ? "file" : "files")} found."
        );

        console.WriteLine();

        var ctx = new FileActionContext<TSettings>(console) { Files = files, Settings = settings };

        int successCount = 0;
        int failCount = 0;
        for (var currentFileIndex = 0; currentFileIndex < files.Count; currentFileIndex++)
        {
            var currentFile = files[currentFileIndex];

            PrintCurrentFile(currentFile, currentFileIndex, files.Count);
            try
            {
                ctx.SetCurrentFile(currentFile, currentFileIndex);
                var success = await action.ProcessTagData(ctx);
                if (success)
                {
                    successCount++;
                    console.MarkupLine("Status: success!");
                }
                else
                {
                    console.MarkupLine("Status: skipped!");
                    continue;
                }
                if (ctx.AbortRequested)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                failCount++;
                console.MarkupLineInterpolated($"Status: [red]error![/]");
                console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                continue;
            }
            console.WriteLine();
        }
        console.MarkupLineInterpolated(
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [red]{failCount}[/] failed."
        );

        return 0;
    }

    private void PrintCurrentFile(string file, int index, int total)
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
}

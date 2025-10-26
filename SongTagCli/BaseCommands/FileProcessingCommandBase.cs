using SongTagCli.Misc;
using SongTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SongTagCli.BaseCommands;

public abstract class FileProcessingSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public required string[] Path { get; set; }
}

public abstract class FileProcessingCommandBase<TSettings>(IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : FileProcessingSettings
{
    protected IAnsiConsole Console => console;

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        await AnsiConsole
            .Status()
            .StartAsync("Processing files...", ctx => ProcessFilesAsync(ctx, settings));

        return 0;
    }

    private async Task ProcessFilesAsync(StatusContext ctx, TSettings settings)
    {
        ctx.Status("Processing files...");
        var files = Helper.GetAllAudioFiles(settings.Path, true);

        var failedFiles = new List<(string File, Exception Error)>();

        Console.MarkupLine($"{files.Count} {(files.Count == 1 ? "file" : "files")} found.");

        int successCount = 0;
        int failCount = 0;
        int index = 0;

        foreach (var file in files)
        {
            index++;
            try
            {
                Console.PrintCurrentFile(file, index, files.Count);
                ctx.Status("Processing...");
                await ProcessFileAsync(ctx, settings, files, file);

                // print file after processing
                Console.PrintTagData(Tagger.ReadTags(file));
                successCount++;
            }
            catch (Exception ex)
            {
                failCount++;
                failedFiles.Add((file, ex));
                console.MarkupLineInterpolated($"[red]Error processing {file}: {ex.ToString()}[/]");
            }
        }

        Console.MarkupLineInterpolated(
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [red]{failCount}[/] failed."
        );

        // List failed files if any
        if (failedFiles.Count > 0)
        {
            Console.WriteLine();
            Console.MarkupLine("[bold red]Failed files:[/]");
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold red]File[/]")
                .AddColumn("[bold yellow]Error[/]");

            foreach (var (file, error) in failedFiles)
            {
                table.AddRow(file.EscapeMarkup(), error.Message.EscapeMarkup());
            }
            Console.Write(table);
        }
    }

    protected abstract Task ProcessFileAsync(
        StatusContext ctx,
        TSettings settings,
        List<string> files,
        string file
    );
}

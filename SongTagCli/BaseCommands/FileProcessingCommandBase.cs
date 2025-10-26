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

    protected virtual bool PrintFileAfterProcessing => true;

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

        var failedFiles = new List<(string File, string Error)>();

        Console.MarkupLine($"{files.Count} {(files.Count == 1 ? "file" : "files")} found.");

        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        int index = 0;

        foreach (var file in files)
        {
            index++;
            ProcessFileResult result;
            try
            {
                Console.PrintCurrentFile(file, index, files.Count);
                ctx.Status("Processing...");
                result = await ProcessFileAsync(ctx, settings, files, file);
                if (!string.IsNullOrEmpty(result.Message))
                {
                    Console.WriteLine(result.Message);
                }
            }
            catch (Exception ex)
            {
                failCount++;
                result = new ProcessFileResult(ex);
            }
            if (result.Status == ProcessFileResultStatus.Skipped)
            {
                skipCount++;
                Console.MarkupLine("Status: skipped!");
            }
            else if (result.Status == ProcessFileResultStatus.Error)
            {
                failCount++;
                failedFiles.Add((file, result.Message ?? ""));
                console.MarkupLineInterpolated($"Status: [red]error![/]");
            }
            else
            {
                successCount++;
                Console.MarkupLine("Status: success!");
            }
            if (!string.IsNullOrEmpty(result.Message))
            {
                Console.WriteLine(result.Message);
            }
            if (PrintFileAfterProcessing)
            {
                try
                {
                    Console.PrintTagData(Tagger.ReadTags(file));
                }
                catch (Exception ex)
                {
#if DEBUG
                    AnsiConsole.WriteException(ex);
#endif
                }
            }
        }

        Console.MarkupLineInterpolated(
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
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
                table.AddRow(file.EscapeMarkup(), error.EscapeMarkup());
            }
            Console.Write(table);
        }
    }

    protected abstract Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        TSettings settings,
        List<string> files,
        string file
    );
}

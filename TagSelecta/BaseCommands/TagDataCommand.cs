using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions;
using TagSelecta.Tagging;

namespace TagSelecta.BaseCommands;

public class TagDataCommand<TAction, TSettings>(TAction action, IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : BaseSettings
    where TAction : ITagDataAction<TSettings>
{
    private bool _allConfirmed = false;

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

        var ctx = new TagDataActionContext<TSettings>(console)
        {
            Files = files,
            Settings = settings,
        };

        var shouldContinue = await action.BeforeProcessTagData(ctx);

        if (!shouldContinue)
        {
            return 0;
        }

        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        for (var currentFileIndex = 0; currentFileIndex < files.Count; currentFileIndex++)
        {
            var currentFile = files[currentFileIndex];

            PrintCurrentFile(currentFile, currentFileIndex, files.Count);
            try
            {
                var tagData = Tagger.ReadTags(currentFile);
                ctx.SetCurrentFile(currentFile, currentFileIndex, tagData);
                var originalTagData = tagData.Clone();
                var status = await action.ProcessTagData(ctx);
                if (status == ActionStatus.Skipped)
                {
                    skipCount++;
                    console.MarkupLine("Status: skipped!");
                    continue;
                }
                else
                {
                    if (
                        action.CompareBeforeWriteTagData
                        && TagDataComparer.TagDataEquals(originalTagData, tagData)
                    )
                    {
                        skipCount++;
                        console.MarkupLine("Nothing to change.");
                        continue;
                    }
                    else
                    {
                        TagDataPrinter.PrintComparison(console, originalTagData, tagData);
                        if (!ConfirmPrompt())
                        {
                            skipCount++;
                            console.MarkupLine("Status: skipped!");
                            continue;
                        }
                        await action.BeforeWriteTagData(ctx);
                        Tagger.WriteTags(currentFile, tagData);
                        successCount++;
                        console.MarkupLine("Status: success!");
                    }
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
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
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

    protected bool ConfirmPrompt()
    {
        if (_allConfirmed)
            return true;

        var confirmation = console.Prompt(
            new TextPrompt<string>("Confirm changes?").AddChoices(["y", "n", "a"]).DefaultValue("y")
        );

        switch (confirmation)
        {
            case "y":
                return true;

            case "n":
                return false;

            case "a":
                _allConfirmed = true;
                return true;

            default:
                return false;
        }
    }
}

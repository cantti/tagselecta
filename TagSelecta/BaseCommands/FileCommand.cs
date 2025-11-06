using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Misc;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.BaseCommands;

public abstract class FileCommand<TSettings>(IAnsiConsole console) : AsyncCommand<TSettings>
    where TSettings : FileSettings
{
    protected IAnsiConsole Console => console;

    private TSettings? _settings;
    private bool _isLastFile;
    private bool _allConfirmed;
    private bool _cancelRequested;
    private bool _skipped;

    protected bool ShowContinue { get; set; }

    protected TSettings Settings =>
        _settings ?? throw new InvalidOperationException("Settings not set");
    protected List<string> Files { get; private set; } = [];

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        _settings = settings;

        Console.MarkupLine("Searching for files...");

        Console.WriteLine();

        Files = FileHelper.GetAllAudioFiles(settings.Path, true);

        Console.MarkupLineInterpolated(
            $"[yellow]{Files.Count}[/] {(Files.Count == 1 ? "file" : "files")} found."
        );

        Console.WriteLine();

        await BeforeExecute();

        if (!_cancelRequested)
        {
            var (successCount, skipCount, failCount) = await ExecuteForFiles(Files);
            Console.MarkupLineInterpolated(
                $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
            );
        }

        return 0;
    }

    protected void Skip()
    {
        _skipped = true;
    }

    protected void Cancel()
    {
        _cancelRequested = true;
    }

    protected bool ConfirmPrompt()
    {
        if (_allConfirmed)
            return true;

        var confirmation = Console.Prompt(
            new TextPrompt<string>("Confirm changes?")
                .AddChoices(["y", "n", "a", "c"])
                .DefaultValue("y")
        );

        var confirmed = false;

        if (confirmation == "y")
        {
            confirmed = true;
        }

        if (confirmation == "n")
        {
            _skipped = true;
        }

        if (confirmation == "a")
        {
            _allConfirmed = true;
            confirmed = true;
        }

        if (confirmation == "c")
        {
            _cancelRequested = true;
        }

        return confirmed;
    }

    protected bool TagDataChanged(TagData tagData1, TagData tagData2)
    {
        if (TagDataComparer.TagDataEquals(tagData1, tagData2))
        {
            Console.MarkupLine("Nothing to change.");
            return false;
        }
        else
        {
            Printer.PrintComparison(Console, tagData1, tagData2);
            return true;
        }
    }

    protected bool ValidateFieldNameList(IEnumerable<string> fields)
    {
        foreach (var tagToKeep in fields)
        {
            if (!ValidateFieldName(tagToKeep))
            {
                Console.MarkupLineInterpolated($"[red]Unknown tag: {tagToKeep}[/]");
                return false;
            }
        }
        return true;
    }

    protected bool ContinuePrompt()
    {
        if (_allConfirmed || _isLastFile)
            return false;

        var confirmation = Console.Prompt(
            new TextPrompt<string>("Continue?").AddChoices(["y", "n"]).DefaultValue("y")
        );

        return confirmation == "y";
    }

    protected bool ValidateFieldName(string field)
    {
        var tagDataProps = typeof(TagData).GetProperties();
        return tagDataProps.Any(x =>
            x.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase)
        );
    }

    protected List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }

    protected virtual Task BeforeExecute()
    {
        return Task.CompletedTask;
    }

    protected abstract Task Execute(string file, int index);

    private async Task<(int SuccessCount, int SkipCount, int FailCount)> ExecuteForFiles(
        List<string> files
    )
    {
        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        int index = -1;
        foreach (var file in files)
        {
            index++;
            _skipped = false;
            _isLastFile = index == files.Count - 1;
            try
            {
                PrintCurrentFile(file, index, files.Count);
                await Execute(file, index);
            }
            catch (Exception ex)
            {
                failCount++;
                Console.MarkupLineInterpolated($"Status: [red]error![/]");
                // Console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                Console.WriteException(ex);
                continue;
            }
            if (_cancelRequested)
            {
                skipCount = files.Count - index;
                break;
            }
            else if (_skipped)
            {
                skipCount++;
                Console.MarkupLine("Status: skipped!");
            }
            else
            {
                successCount++;
                Console.MarkupLine("Status: success!");
                if (ShowContinue && !ContinuePrompt())
                {
                    skipCount = files.Count - index - 1;
                    break;
                }
            }
            Console.WriteLine();
        }
        return (successCount, skipCount, failCount);
    }

    private void PrintCurrentFile(string file, int index, int total)
    {
        Console.MarkupInterpolated($"[dim]>[/] [yellow]({index + 1}/{total})[/] \"");
        var path = new TextPath(file)
            .RootColor(Color.White)
            .SeparatorColor(Color.White)
            .StemColor(Color.White)
            .LeafColor(Color.Yellow);
        Console.Write(path);
        Console.Write("\"");
        Console.WriteLine();
    }
}

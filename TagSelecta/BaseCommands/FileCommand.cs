using Spectre.Console;

namespace TagSelecta.BaseCommands;

// command to work with file by file
public abstract class FileCommand<TSettings>(IAnsiConsole console) : BaseCommand<TSettings>(console)
    where TSettings : BaseSettings
{
    private bool _allConfirmed;
    private bool _cancelRequested;
    private bool _skipped;

    protected string CurrentFile { get; set; } = "";

    protected int CurrentFileIndex { get; set; }

    protected sealed override void Process() { }

    protected sealed override async Task ProcessAsync()
    {
        await BeforeProcessAsync();
        var (successCount, skipCount, failCount) = await ProcessFiles();
        Console.MarkupLineInterpolated(
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
        );
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
        {
            return true;
        }

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

    protected virtual void ProcessFile() { }

    protected virtual Task ProcessFileAsync()
    {
        ProcessFile();
        return Task.CompletedTask;
    }

    protected virtual void BeforeProcess() { }

    protected virtual Task BeforeProcessAsync()
    {
        BeforeProcess();
        return Task.CompletedTask;
    }

    private async Task<(int SuccessCount, int SkipCount, int FailCount)> ProcessFiles()
    {
        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        for (CurrentFileIndex = 0; CurrentFileIndex < Files.Count; CurrentFileIndex++)
        {
            CurrentFile = Files[CurrentFileIndex];
            _skipped = false;
            PrintCurrentFile(CurrentFile, CurrentFileIndex, Files.Count);
            try
            {
                await ProcessFileAsync();
            }
            catch (Exception ex)
            {
                failCount++;
                Console.MarkupLineInterpolated($"Status: [red]error![/]");
                Console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                continue;
            }
            if (_cancelRequested)
            {
                skipCount = Files.Count - CurrentFileIndex;
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
            }
            Console.WriteLine();
        }
        return (successCount, skipCount, failCount);
    }
}

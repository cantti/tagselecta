using Spectre.Console;

namespace TagSelecta.BaseCommands;

public abstract class FileProcessingCommand<TSettings>(IAnsiConsole console)
    : BaseCommand<TSettings>(console)
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
            return true;

        var confirmation = Console.Prompt(
            new TextPrompt<string>("Confirm changes? (yes, no, yes to all, cancel)")
                .AddChoices(["y", "n", "a", "c"])
                .DefaultValue("y")
        );

        switch (confirmation)
        {
            case "y":
                return true;

            case "n":
                _skipped = true;
                return false;

            case "a":
                _allConfirmed = true;
                return true;

            case "c":
                _cancelRequested = true;
                return false;

            default:
                return false;
        }
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
}

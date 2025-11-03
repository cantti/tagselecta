using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.Misc;
using TagSelecta.Print;

namespace TagSelecta.BaseCommands;

public sealed class FileCommand<TAction, TSettings>(
    IAnsiConsole console,
    Printer printer,
    FileActionFactory<TAction, TSettings> actionFactory
) : AsyncCommand<TSettings>
    where TAction : IFileAction<TSettings>
    where TSettings : FileSettings
{
    private bool _isLastFile;
    private bool _allConfirmed;
    private bool _cancelRequested;
    private bool _skipped;
    private readonly ActionConfig _config = new();

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        console.MarkupLine("Searching for files...");

        console.WriteLine();

        var files = FileHelper.GetAllAudioFiles(settings.Path, true);

        var actionContext = new ActionContext<TSettings>()
        {
            ConfirmPrompt = ConfirmPrompt,
            Cancel = Cancel,
            Skip = Skip,
            Files = files,
            Settings = settings,
        };

        var action = actionFactory.Create(actionContext);

        action.Configure(_config);

        console.MarkupLineInterpolated(
            $"[yellow]{files.Count}[/] {(files.Count == 1 ? "file" : "files")} found."
        );

        console.WriteLine();

        await action.BeforeExecute();

        if (!_cancelRequested)
        {
            var (successCount, skipCount, failCount) = await ExecuteForFiles(files, action);
            console.MarkupLineInterpolated(
                $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
            );
        }

        return 0;
    }

    private async Task<(int SuccessCount, int SkipCount, int FailCount)> ExecuteForFiles(
        List<string> files,
        IFileAction<TSettings> action
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
                printer.PrintCurrentFile(file, index, files.Count);
                await action.Execute(file, index);
            }
            catch (Exception ex)
            {
                failCount++;
                console.MarkupLineInterpolated($"Status: [red]error![/]");
                console.WriteException(ex);
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
                console.MarkupLine("Status: skipped!");
            }
            else
            {
                successCount++;
                console.MarkupLine("Status: success!");
                if (_config.ShowContinue && !ContinuePrompt())
                {
                    skipCount = files.Count - index - 1;
                    break;
                }
            }
            console.WriteLine();
        }
        return (successCount, skipCount, failCount);
    }

    public void Skip()
    {
        _skipped = true;
    }

    public void Cancel()
    {
        _cancelRequested = true;
    }

    bool ConfirmPrompt()
    {
        if (_allConfirmed)
            return true;

        var confirmation = console.Prompt(
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

    private bool ContinuePrompt()
    {
        if (_allConfirmed || _isLastFile)
            return false;

        var confirmation = console.Prompt(
            new TextPrompt<string>("Continue?").AddChoices(["y", "n"]).DefaultValue("y")
        );

        return confirmation == "y";
    }
}

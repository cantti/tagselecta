using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.Misc;
using TagSelecta.Print;

namespace TagSelecta.BaseCommands;

public sealed class FileCommand<TSettings>(IAnsiConsole console, IAction<TSettings> action)
    : AsyncCommand<TSettings>
    where TSettings : FileSettings
{
    private bool _isLastFile;
    private bool _allConfirmed;
    private ExecuteStatus _executeStatus;

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var files = FileHelper.GetAllAudioFiles(settings.Path, true);

        console.MarkupLine($"{files.Count} {(files.Count == 1 ? "file" : "files")} found.");

        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        int index = -1;

        foreach (var file in files)
        {
            _executeStatus = ExecuteStatus.Success;
            index++;
            _isLastFile = index == files.Count - 1;
            try
            {
                console.PrintCurrentFile(file, index, files.Count);
                action.Execute(
                    new ActionContext<TSettings>()
                    {
                        ConfirmPrompt = ConfirmPrompt,
                        ContinuePrompt = ContinuePrompt,
                        Skip = Skip,
                        File = file,
                        Files = files,
                        Settings = settings,
                        Console = console,
                    }
                );
            }
            catch (Exception ex)
            {
                failCount++;
                console.MarkupLineInterpolated($"Status: [red]error![/]");
                console.WriteException(ex);
                continue;
            }
            if (_executeStatus == ExecuteStatus.CancelRequested)
            {
                break;
            }
            else if (_executeStatus == ExecuteStatus.DoNotContinueRequested)
            {
                successCount++;
                skipCount = files.Count - index - 1;
                break;
            }
            else if (_executeStatus == ExecuteStatus.Skipped)
            {
                skipCount++;
                console.MarkupLine("Status: skipped!");
            }
            else
            {
                successCount++;
                console.MarkupLine("Status: success!");
            }
        }

        console.MarkupLineInterpolated(
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
        );

        return 0;
    }

    public void Skip()
    {
        _executeStatus = ExecuteStatus.Skipped;
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
            _executeStatus = ExecuteStatus.Skipped;
        }

        if (confirmation == "a")
        {
            _allConfirmed = true;
            confirmed = true;
        }

        if (confirmation == "c")
        {
            _executeStatus = ExecuteStatus.CancelRequested;
        }

        return confirmed;
    }

    void ContinuePrompt()
    {
        if (_allConfirmed || _isLastFile)
            return;

        var confirmation = console.Prompt(
            new TextPrompt<string>("Continue?").AddChoices(["y", "n"]).DefaultValue("y")
        );

        if (confirmation == "n")
        {
            _executeStatus = ExecuteStatus.DoNotContinueRequested;
        }
    }
}

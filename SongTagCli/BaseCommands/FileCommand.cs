using SongTagCli.Actions.Base;
using SongTagCli.Misc;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SongTagCli.BaseCommands;

public sealed class FileCommand<TSettings, TAction>(IAnsiConsole console) : AsyncCommand<TSettings>
    where TSettings : FileSettings
    where TAction : IAction<TSettings>, new()
{
    private bool _allConfirmed;
    private bool _canceled;
    private bool _skipped;
    private bool _isLastFile;
    readonly TAction _action = new();

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
            index++;
            _isLastFile = index == files.Count - 1;
            try
            {
                console.PrintCurrentFile(file, index, files.Count);
                _action.Execute(
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
            if (_canceled)
            {
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
            }
        }

        console.MarkupLineInterpolated(
            $"[green]Finished![/] Processed [yellow]{successCount}[/] files, [cyan]{skipCount}[/] skipped, [red]{failCount}[/] failed."
        );

        return 0;
    }

    public void Skip()
    {
        _skipped = true;
    }

    bool ConfirmPrompt()
    {
        _skipped = false;

        if (_allConfirmed)
            return true;

        var choices = new Dictionary<ConfirmChoice, string>
        {
            { ConfirmChoice.Yes, "y" },
            { ConfirmChoice.No, "n" },
            { ConfirmChoice.All, "a" },
            { ConfirmChoice.Cancel, "c" },
        };

        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<ConfirmChoice>("Confirm changes?")
                .AddChoices(choices.Keys)
                .DefaultValue(ConfirmChoice.Yes)
                .WithConverter(choice => choices[choice])
        );

        switch (confirmation)
        {
            case ConfirmChoice.Yes:
                return true;

            case ConfirmChoice.No:
                _skipped = true;
                return false;

            case ConfirmChoice.All:
                _allConfirmed = true;
                return true;

            case ConfirmChoice.Cancel:
                _canceled = true;
                return false;

            default:
                return false;
        }
    }

    void ContinuePrompt()
    {
        if (_allConfirmed || _isLastFile)
            return;

        var choices = new Dictionary<ConfirmChoice, string>
        {
            { ConfirmChoice.Yes, "y" },
            { ConfirmChoice.No, "n" },
        };

        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<ConfirmChoice>("Continue?")
                .AddChoices(choices.Keys)
                .DefaultValue(ConfirmChoice.Yes)
                .WithConverter(choice => choices[choice])
        );

        if (confirmation == ConfirmChoice.No)
        {
            _canceled = true;
        }
    }
}

using SongTagCli.Misc;
using SongTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SongTagCli.BaseCommands;

public abstract class FileProcessingSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];
}

public abstract class FileProcessingCommandBase<TSettings>(IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : FileProcessingSettings
{
    protected IAnsiConsole Console => console;
    private bool _allConfirmed;
    private bool _canceled;
    private bool _isLast = false;

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var files = Helper.GetAllAudioFiles(settings.Path, true);

        var failedFiles = new List<(string File, string Error)>();

        Console.MarkupLine($"{files.Count} {(files.Count == 1 ? "file" : "files")} found.");

        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        int index = -1;

        foreach (var file in files)
        {
            index++;
            _isLast = index == files.Count - 1;
            ResultStatus result;
            try
            {
                Console.PrintCurrentFile(file, index, files.Count);
                result = await ProcessFileAsync(settings, files, file);
            }
            catch (Exception ex)
            {
                failCount++;
                result = ResultStatus.Error;
                Console.WriteException(ex);
            }
            if (_canceled)
            {
                skipCount++;
                break;
            }
            if (result == ResultStatus.Skipped)
            {
                skipCount++;
                Console.MarkupLine("Status: skipped!");
            }
            else if (result == ResultStatus.Error)
            {
                failCount++;
                console.MarkupLineInterpolated($"Status: [red]error![/]");
            }
            else
            {
                successCount++;
                Console.MarkupLine("Status: success!");
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

        return 0;
    }

    protected bool Continue()
    {
        if (_allConfirmed || _isLast)
            return true;

        var choices = new Dictionary<ConfirmChoice, string>
        {
            { ConfirmChoice.Yes, "y" },
            { ConfirmChoice.No, "n" },
            { ConfirmChoice.All, "a" },
        };

        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<ConfirmChoice>("Continue?")
                .AddChoices(choices.Keys)
                .DefaultValue(ConfirmChoice.Yes)
                .WithConverter(choice => choices[choice])
        );

        switch (confirmation)
        {
            case ConfirmChoice.Yes:
                return true;

            case ConfirmChoice.No:
                _canceled = true;
                return false;

            case ConfirmChoice.All:
                _allConfirmed = true;
                return true;

            default:
                return false;
        }
    }

    protected bool Confirm()
    {
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

    protected void PrintTagData(TagData tagData)
    {
        Console.PrintTagData(tagData);
    }

    protected abstract Task<ResultStatus> ProcessFileAsync(
        TSettings settings,
        List<string> files,
        string file
    );
}

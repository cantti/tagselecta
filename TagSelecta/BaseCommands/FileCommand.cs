using System.Reflection;
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

    private bool _isLastFile;
    private bool _allConfirmed;
    private bool _cancelRequested;
    private bool _skipped;

    protected bool ShowContinue { get; set; }

    private TSettings? _settings;
    protected TSettings Settings =>
        _settings ?? throw new InvalidOperationException("_settings not set");

    private TagData? _tagData;
    protected TagData TagData =>
        _tagData ?? throw new InvalidOperationException("_tagData not set");

    private TagData? _originalTagData;
    private TagData OriginalTagData =>
        _originalTagData ?? throw new InvalidOperationException("_originalTagData not set");

    protected List<string> Files { get; private set; } = [];

    protected string CurrentFile { get; set; } = "";

    protected int CurrentileIndex { get; set; } = -1;

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

        await BeforeExecuteAsync();

        if (!_cancelRequested)
        {
            var (successCount, skipCount, failCount) = await ExecuteForFiles();
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

    protected void WriteTags(bool removeExisting = false)
    {
        var changed = false;
        if (TagDataComparer.TagDataEquals(OriginalTagData, TagData))
        {
            Console.MarkupLine("Nothing to change.");
        }
        else
        {
            Printer.PrintComparison(Console, OriginalTagData, TagData);
            changed = true;
        }
        if (changed && ConfirmPrompt())
        {
            if (removeExisting)
            {
                Tagger.RemoveTags(CurrentFile);
            }
            Tagger.WriteTags(CurrentFile, TagData);
        }
    }

    protected bool ValidateFieldNameList(IEnumerable<string> fields)
    {
        var tagDataProps = typeof(TagData)
            .GetProperties()
            .Where(x => x.GetCustomAttribute<EditableAttribute>() != null);
        foreach (var field in fields)
        {
            if (
                !tagDataProps.Any(x =>
                    x.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase)
                )
            )
            {
                Console.MarkupLineInterpolated($"[red]Unknown field: {field}[/]");
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

    protected List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }

    protected virtual void BeforeExecute() { }

    protected virtual Task BeforeExecuteAsync()
    {
        BeforeExecute();
        return Task.CompletedTask;
    }

    protected virtual void Execute() { }

    protected virtual Task ExecuteAsync()
    {
        Execute();
        return Task.CompletedTask;
    }

    // protected virtual Task Execute(string file, int index);

    private async Task<(int SuccessCount, int SkipCount, int FailCount)> ExecuteForFiles()
    {
        int successCount = 0;
        int failCount = 0;
        int skipCount = 0;
        foreach (var file in Files)
        {
            CurrentileIndex++;
            _skipped = false;
            _isLastFile = CurrentileIndex == Files.Count - 1;
            _tagData = Tagger.ReadTags(file);
            _originalTagData = _tagData.Clone();
            CurrentFile = file;
            try
            {
                PrintCurrentFile(file, CurrentileIndex, Files.Count);
                await ExecuteAsync();
            }
            catch (Exception ex)
            {
                failCount++;
                Console.MarkupLineInterpolated($"Status: [red]error![/]");
                Console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                // Console.WriteException(ex);
                continue;
            }
            if (_cancelRequested)
            {
                skipCount = Files.Count - CurrentileIndex;
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
                    skipCount = Files.Count - CurrentileIndex - 1;
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

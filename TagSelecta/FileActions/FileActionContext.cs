using Spectre.Console;

namespace TagSelecta.FileActions;

public class FileActionContext<TSettings>(IAnsiConsole console)
{
    private bool _allConfirmed = false;

    public bool AbortRequested { get; private set; }

    public required List<string> Files { get; set; }

    public required TSettings Settings { get; set; }

    private string? _currentFile;
    public string CurrentFile => _currentFile ?? throw new InvalidOperationException();

    private int? _currentFileIndex;
    public int CurrentFileIndex => _currentFileIndex ?? throw new InvalidOperationException();

    public void SetCurrentFile(string currentFile, int currentFileIndex)
    {
        _currentFile = currentFile;
        _currentFileIndex = currentFileIndex;
    }

    public bool ConfirmPrompt()
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

    public void ContinuePrompt()
    {
        if (_allConfirmed)
            return;

        var confirmation = console.Prompt(
            new TextPrompt<string>("Continue? (yes, no, yes to all)")
                .AddChoices(["y", "n", "a"])
                .DefaultValue("y")
        );

        switch (confirmation)
        {
            case "y":
                return;

            case "a":
                _allConfirmed = true;
                return;

            case "n":
                AbortRequested = true;
                return;

            default:
                return;
        }
    }
}

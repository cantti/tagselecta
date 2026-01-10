namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedOperationIndex { get; set; }
    IEnumerable<TagDataOperation> SelectedOperations { get; }
    TagDataOperation? FocusedOperation { get; }
    bool TreeEnabled { get; set; }
    bool FilterEnabled { get; set; }
    bool HelpEnabled { get; set; }
    void Quit();
    void Print(string markupMessage);
}

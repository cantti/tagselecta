namespace TagSelecta.App.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedOperationIndex { get; set; }
    IEnumerable<TagDataOperation> Operations { get; }
    IEnumerable<TagDataOperation> SelectedOperations { get; }
    TagDataOperation? FocusedOperation { get; }
    bool TreeEnabled { get; set; }
    bool FilterEnabled { get; set; }
    bool HelpEnabled { get; set; }
    IEnumerable<TagDataOperation> VisibleOperations { get; }
    void Quit();
    void Print(string markupMessage);
}

namespace TagSelecta.Cli.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedOperationIndex { get; set; }
    List<TagDataOperation> Operations { get; }
    void Quit();
}

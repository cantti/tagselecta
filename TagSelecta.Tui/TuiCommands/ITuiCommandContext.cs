using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedFileIndex { get; set; }
    IEnumerable<TagDataActionTarget> Files { get; }
    IEnumerable<TagDataActionTarget> SelectedFiles { get; }
    TagDataActionTarget? FocusedFile { get; }
    bool TreeEnabled { get; set; }
    bool FilterEnabled { get; set; }
    bool HelpEnabled { get; set; }
    IEnumerable<TagDataActionTarget> VisibleFiles { get; }
    void Quit();
    void Print(string markupMessage);
}

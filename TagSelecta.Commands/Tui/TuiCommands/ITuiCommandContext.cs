using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedFileIndex { get; set; }
    IEnumerable<TagDataActionTarget> Files { get; }
    IEnumerable<TagDataActionTarget> SelectedFiles { get; }
    TagDataActionTarget? FocusedFile { get; }
    bool TreeEnabled { get; set; }
    bool FilterEnabled { get; set; }
    bool KeymapHelpEnabled { get; set; }
    bool CommandHelpEnabled { get; set; }
    bool FileListEnabled { get; set; }
    IEnumerable<TagDataActionTarget> VisibleFiles { get; }
    bool PictureEnabled { get; set; }
    ITuiCommandFactory CommandFactory { get; }
    void Quit();
    void Print(string markupMessage);
    void SetCommandPromptText(string text);
    void SetFiles(IEnumerable<TagDataActionTarget> files);
}

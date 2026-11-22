namespace TagSelecta.Commands.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedFileIndex { get; set; }
    int MetadataOffset { get; set; }
    IEnumerable<TagDataActionTarget> Files { get; }
    IEnumerable<TagDataActionTarget> SelectedFiles { get; }
    TagDataActionTarget? FocusedFile { get; }
    bool TreeEnabled { get; set; }
    bool FilterEnabled { get; set; }
    bool CommandHelpEnabled { get; set; }
    string HelpTopic { get; set; }
    bool FileListEnabled { get; set; }
    bool MetadataEnabled { get; set; }
    IEnumerable<TagDataActionTarget> VisibleFiles { get; }
    bool PictureEnabled { get; set; }
    void Quit();
    void Print(string markupMessage);
    void SetCommandPromptText(string text);
    void SetFiles(IEnumerable<TagDataActionTarget> files);
}

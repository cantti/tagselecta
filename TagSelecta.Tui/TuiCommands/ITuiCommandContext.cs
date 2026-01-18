using TagSelecta.Shared.TrackedFiles;

namespace TagSelecta.Tui.TuiCommands;

public interface ITuiCommandContext
{
    int FocusedFileIndex { get; set; }
    IEnumerable<TrackedFile> Files { get; }
    IEnumerable<TrackedFile> SelectedFiles { get; }
    TrackedFile? FocusedFile { get; }
    bool TreeEnabled { get; set; }
    bool FilterEnabled { get; set; }
    bool HelpEnabled { get; set; }
    IEnumerable<TrackedFile> VisibleFiles { get; }
    void Quit();
    void Print(string markupMessage);
}

namespace TagSelecta.App.TagDataActions.FileCommands;

public interface IFileActionContext<TSettings>
{
    List<string> Files { get; set; }
    TSettings Settings { get; set; }
    string CurrentFile { get; }
    int CurrentFileIndex { get; }

    bool ConfirmPrompt();
    void SetCurrentFile(string currentFile, int currentFileIndex);
}

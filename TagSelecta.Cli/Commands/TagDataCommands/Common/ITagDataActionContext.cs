using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands.Common;

public interface ITagDataActionContext<TSettings>
{
    List<string> Files { get; set; }
    TSettings Settings { get; set; }
    string CurrentFile { get; }
    int CurrentFileIndex { get; }
    TagData TagData { get; }
}

using TagSelecta.Cli.Tui;

namespace TagSelecta.Cli.Commands.AutoTrack;

[TagDataAction("autotrack", "at")]
public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void ProcessTagData(
        FileWithTagData current,
        List<FileWithTagData> files,
        AutoTrackSettings settings
    )
    {
        var dir = Directory.GetParent(current.Path)?.FullName;
        var filesInDir = files
            .Select(x => x.Path)
            .Where(x => Directory.GetParent(x)?.FullName == dir)
            .Order()
            .ToList();
        current.TagData.Track = (filesInDir.IndexOf(current.Path) + 1).ToString();
        current.TagData.TrackTotal = filesInDir.Count.ToString();
        if (!settings.KeepDisk)
        {
            current.TagData.Disc = "";
            current.TagData.DiscTotal = "";
        }
    }
}

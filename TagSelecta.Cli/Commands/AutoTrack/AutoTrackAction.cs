using TagSelecta.Cli.Commands.Common;

namespace TagSelecta.Cli.Commands.AutoTrack;

public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void ProcessTagData(
        TagDataOperation current,
        List<TagDataOperation> operations,
        AutoTrackSettings settings
    )
    {
        var dir = Directory.GetParent(current.Path)?.FullName;
        var filesInDir = operations
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

using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.TagDataCommands.Common;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class AutoTrackSettings : BaseSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void ProcessTagData(
        Item current,
        List<Item> items,
        AutoTrackSettings settings
    )
    {
        var dir = Directory.GetParent(current.Path)?.FullName;
        var filesInDir = items
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

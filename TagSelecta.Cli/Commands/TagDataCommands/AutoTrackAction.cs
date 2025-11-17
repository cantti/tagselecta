using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class AutoTrackSettings : BaseSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void ProcessTagData(TagDataActionContext<AutoTrackSettings> context)
    {
        var dir = Directory.GetParent(context.CurrentFile)?.FullName;
        var filesInDir = context
            .Files.Where(x => Directory.GetParent(x)?.FullName == dir)
            .Order()
            .ToList();
        context.TagData.Track = (filesInDir.IndexOf(context.CurrentFile) + 1).ToString();
        context.TagData.TrackTotal = filesInDir.Count.ToString();
        if (!context.Settings.KeepDisk)
        {
            context.TagData.Disc = "";
            context.TagData.DiscTotal = "";
        }
    }
}

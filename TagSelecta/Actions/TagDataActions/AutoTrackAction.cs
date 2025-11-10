using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.Actions.TagDataActions;

public class AutoTrackSettings : BaseSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackAction : ITagDataAction<AutoTrackSettings>
{
    public Task ProcessTagData(TagDataActionContext<AutoTrackSettings> context)
    {
        var dir = Directory.GetParent(context.CurrentFile)?.FullName;
        var filesInDir = context
            .Files.Where(x => Directory.GetParent(x)?.FullName == dir)
            .Order()
            .ToList();
        context.TagData.Track = (uint)filesInDir.IndexOf(context.CurrentFile) + 1;
        context.TagData.TrackTotal = (uint)filesInDir.Count;
        if (!context.Settings.KeepDisk)
        {
            context.TagData.Disc = 0;
            context.TagData.DiscTotal = 0;
        }
        return Task.CompletedTask;
    }
}

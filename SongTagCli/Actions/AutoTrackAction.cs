using System.ComponentModel;
using SongTagCli.Actions.Base;
using SongTagCli.BaseCommands;
using SongTagCli.Misc;
using SongTagCli.Tagging;
using Spectre.Console.Cli;

namespace SongTagCli.Actions;

public class AutoTrackSettings : FileSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackAction : IAction<AutoTrackSettings>
{
    public void Execute(ActionContext<AutoTrackSettings> context)
    {
        var dir = Directory.GetParent(context.File)?.FullName;
        var filesInDir = context
            .Files.Where(x => Directory.GetParent(x)?.FullName == dir)
            .Order()
            .ToList();
        var tags = Tagger.ReadTags(context.File);
        tags.Track = (uint)filesInDir.IndexOf(context.File) + 1;
        tags.TrackTotal = (uint)filesInDir.Count;
        if (!context.Settings.KeepDisk)
        {
            tags.Disc = 0;
            tags.DiscTotal = 0;
        }
        context.Console.PrintTagData(tags);
        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
        }
    }
}

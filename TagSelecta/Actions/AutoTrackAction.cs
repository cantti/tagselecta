using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class AutoTrackSettings : FileSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackAction(Printer printer) : FileAction<AutoTrackSettings>
{
    public override void Execute(ActionContext<AutoTrackSettings> context)
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
        printer.PrintTagData(tags);
        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
        }
    }
}

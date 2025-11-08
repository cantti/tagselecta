using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class AutoTrackSettings : FileSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackCommand(IAnsiConsole console) : FileCommand<AutoTrackSettings>(console)
{
    protected override void Execute()
    {
        var dir = Directory.GetParent(CurrentFile)?.FullName;
        var filesInDir = Files.Where(x => Directory.GetParent(x)?.FullName == dir).Order().ToList();
        TagData.Track = (uint)filesInDir.IndexOf(CurrentFile) + 1;
        TagData.TrackTotal = (uint)filesInDir.Count;
        if (!Settings.KeepDisk)
        {
            TagData.Disc = 0;
            TagData.DiscTotal = 0;
        }
        WriteTags();
    }
}

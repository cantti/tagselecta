using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.App.TagDataActions.AutoTrack;

public class AutoTrackSettings : BaseSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.AutoTrack;

public class AutoTrackSettings : TagDataActionSettings
{
    [CommandOption("--keepdisk")]
    [Description("Keep disc number and disc total")]
    public bool KeepDisk { get; set; }
}

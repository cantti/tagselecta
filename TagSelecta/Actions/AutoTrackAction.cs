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

public class AutoTrackAction(Printer printer, ActionContext<AutoTrackSettings> context)
    : IFileAction<AutoTrackSettings>
{
    public Task Execute(string file, int index)
    {
        var dir = Directory.GetParent(file)?.FullName;
        var filesInDir = context
            .Files.Where(x => Directory.GetParent(x)?.FullName == dir)
            .Order()
            .ToList();
        var tags = Tagger.ReadTags(file);
        tags.Track = (uint)filesInDir.IndexOf(file) + 1;
        tags.TrackTotal = (uint)filesInDir.Count;
        if (!context.Settings.KeepDisk)
        {
            tags.Disc = 0;
            tags.DiscTotal = 0;
        }
        printer.PrintTagData(tags);
        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }
        return Task.CompletedTask;
    }
}

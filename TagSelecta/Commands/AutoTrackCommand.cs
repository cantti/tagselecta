using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
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
    protected override Task Execute(string file, int index)
    {
        var dir = Directory.GetParent(file)?.FullName;
        var filesInDir = Files.Where(x => Directory.GetParent(x)?.FullName == dir).Order().ToList();
        var tags = Tagger.ReadTags(file);
        tags.Track = (uint)filesInDir.IndexOf(file) + 1;
        tags.TrackTotal = (uint)filesInDir.Count;
        if (!Settings.KeepDisk)
        {
            tags.Disc = 0;
            tags.DiscTotal = 0;
        }
        Printer.PrintTagData(Console, tags);
        if (ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }
        return Task.CompletedTask;
    }
}

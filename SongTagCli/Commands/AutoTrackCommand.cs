using System.ComponentModel;
using SongTagCli.BaseCommands;
using SongTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SongTagCli.Commands;

public class AutoTrackSettings : FileProcessingSettings
{
    [CommandOption("--keepdisk")]
    [Description("Remove Disc and DiscTotal")]
    public bool KeepDisk { get; set; }
}

public class AutoTrackCommand(IAnsiConsole console)
    : FileProcessingCommandBase<AutoTrackSettings>(console)
{
    protected override async Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        AutoTrackSettings settings,
        List<string> files,
        string file
    )
    {
        var dir = Directory.GetParent(file)?.FullName;
        var filesInDir = files.Where(x => Directory.GetParent(x)?.FullName == dir).Order().ToList();
        var tags = Tagger.ReadTags(file);
        tags.Track = (uint)filesInDir.IndexOf(file) + 1;
        tags.TrackTotal = (uint)filesInDir.Count;
        if (!settings.KeepDisk)
        {
            tags.Disc = 0;
            tags.DiscTotal = 0;
        }
        Tagger.WriteTags(file, tags);
        return await Task.FromResult(ProcessFileResult.Success());
    }
}

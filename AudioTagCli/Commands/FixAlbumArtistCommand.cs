using System.ComponentModel;
using AudioTagCli.Misc;
using AudioTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AudioTagCli.Commands;

public class AddSettings : CommandSettings
{
    [CommandOption("-p|--path", true)]
    public required string Path { get; set; }

    [CommandOption("-a|--apply")]
    [Description("Commit changes")]
    public bool Apply { get; set; }
}

public class FixAlbumArtistCommand : Command<AddSettings>
{
    public override int Execute(CommandContext context, AddSettings settings)
    {
        var files = Helper.GetAllAudioFiles(settings.Path, true);

        foreach (var file in files)
        {
            AnsiConsole.MarkupLineInterpolated($"[blue]Current file: {file}[/]");
            try
            {
                var tags = Tagger.ReadTags(file);
                Console.WriteLine(tags.ToString());
                var hasAlbumArtists = !string.IsNullOrEmpty(tags.AlbumArtist.FirstOrDefault());
                var albumArtistsAreTheSame = false;
                if (hasAlbumArtists)
                {
                    AnsiConsole.MarkupLine($"Album artist found: {tags.AlbumArtistJoined}");
                    albumArtistsAreTheSame = VerifyAllTheSameAlbumArtist(file);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Album artist not found.[/]");
                }
                if (!hasAlbumArtists || !albumArtistsAreTheSame)
                {
                    var newAlbumArtists = GenerateNewAlbumArtistsValue(file);
                    tags.AlbumArtist = newAlbumArtists;
                    if (settings.Apply)
                    {
                        Tagger.WriteTags(file, tags);
                        AnsiConsole.MarkupLine($"[green]New value has been set.[/]");
                        AnsiConsole.MarkupLine("[blue]Updated tags[/]");
                        Console.WriteLine(Tagger.ReadTags(file).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]{ex.ToString()}[/]");
            }
            Console.WriteLine();
        }
        return 0;
    }

    private static bool VerifyAllTheSameAlbumArtist(string file)
    {
        var parentDir = Directory.GetParent(file)!.FullName;
        var filesInSameDir = Helper.GetAllAudioFiles(parentDir, false).Order().ToList();
        AnsiConsole.WriteLine($"Files in the same directory: {filesInSameDir.Count}");
        var albumArtists = new List<string>();
        foreach (var file2 in filesInSameDir)
        {
            albumArtists.Add(Tagger.ReadTags(file2).AlbumArtistJoined);
        }
        var correct = albumArtists.Distinct().Count() == 1;
        if (correct)
        {
            AnsiConsole.WriteLine("Album artists are the same");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Album artists are not the same[/]");
        }
        return correct;
    }

    private static List<string> GenerateNewAlbumArtistsValue(string file)
    {
        var parentDir = Directory.GetParent(file)!.FullName;
        var filesInSameDir = Helper.GetAllAudioFiles(parentDir, false).Order().ToList();
        AnsiConsole.WriteLine($"Files in the same directory: {filesInSameDir.Count}");
        var albumArtists = new List<string>();
        foreach (var file2 in filesInSameDir)
        {
            albumArtists.AddRange(Tagger.ReadTags(file2).Artist);
        }
        albumArtists = [.. albumArtists.Distinct()];
        AnsiConsole.WriteLine($"Distinct artists: {albumArtists.Count}");
        if (albumArtists.Count > 3)
        {
            albumArtists = ["Various Artists"];
        }
        AnsiConsole.WriteLine(
            $"Suggested new album artist name: {string.Join("; ", albumArtists)}"
        );
        return albumArtists;
    }
}

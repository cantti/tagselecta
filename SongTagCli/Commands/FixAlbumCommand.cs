using System.Text;
using SongTagCli.BaseCommands;
using SongTagCli.Misc;
using SongTagCli.Tagging;
using Spectre.Console;

namespace SongTagCli.Commands;

public class FixAlbumSettings : FileProcessingSettings { }

public class FixAlbumCommand(IAnsiConsole console)
    : FileProcessingCommandBase<FixAlbumSettings>(console)
{
    private enum FixType
    {
        PrimaryArtists,
        AllArtists,
        VariousArtists,
    }

    private class Album
    {
        public required string Dir { get; set; }
        public required FixType FixType { get; set; }
        public required List<string> AlbumArtist { get; set; } = [];
        public required string AlbumName { get; set; }
        public required uint Year { get; set; }
    }

    private readonly List<Album> _albums = [];

    protected override async Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        FixAlbumSettings settings,
        List<string> files,
        string file
    )
    {
        var dir = Directory.GetParent(file)!.FullName;
        var album = _albums.SingleOrDefault(x => x.Dir == dir);
        if (album is null)
        {
            var filesInDir = files
                .Where(x => Directory.GetParent(x)?.FullName == dir)
                .Order()
                .ToList();
            var dirTagData = new List<TagData>();
            foreach (var fileInDir in filesInDir)
            {
                dirTagData.Add(Tagger.ReadTags(fileInDir));
            }

            // find most common album name in dir
            var albumName =
                dirTagData
                    .Select(x => x.Album)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .GroupBy(x => x)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()
                    ?.Key
                ?? "";

            // find most common album year in dir
            var albumYear = dirTagData
                .Select(x => x.Year)
                .Where(x => x != 0)
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;

            // get all artists on album
            var artistList = dirTagData
                .SelectMany(x => x.Artist)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .Order()
                .ToList();

            var albumArtist = new List<string>();

            // found artists that exist on all songs
            var primaryArtist = artistList
                .Where(x => dirTagData.All(x2 => x2.Artist.Contains(x)))
                .ToList();

            FixType fixType;

            if (primaryArtist.Count != 0)
            {
                fixType = FixType.PrimaryArtists;
                albumArtist = primaryArtist;
            }
            else if (artistList.Count < 4)
            {
                fixType = FixType.AllArtists;
                albumArtist = artistList;
            }
            else
            {
                fixType = FixType.VariousArtists;
                albumArtist = [Constants.VariousArtists];
            }
            album = new()
            {
                FixType = fixType,
                Dir = dir,
                AlbumName = albumName,
                AlbumArtist = albumArtist,
                Year = albumYear,
            };
            _albums.Add(album);
        }
        var sb = new StringBuilder();
        string albumArtistMessage = album.FixType switch
        {
            FixType.PrimaryArtists =>
                $"Primary artist(s) identified: [yellow]{album.AlbumArtist.Print().EscapeMarkup()}[/]",
            FixType.AllArtists =>
                $"No primary artist(s) detected. Using all contributing artists: [yellow]{album.AlbumArtist.Print().EscapeMarkup()}[/]",
            FixType.VariousArtists =>
                $"Multiple distinct artists detected. Assigning album artist as: [yellow]{album.AlbumArtist.Print().EscapeMarkup()}[/]",
            _ => "",
        };
        sb.AppendLine(albumArtistMessage);
        sb.AppendLine($"The most common album mame: [yellow]{album.AlbumName.EscapeMarkup()}[/]");
        sb.AppendLine($"The most common album year: [yellow]{album.Year}[/]");
        var tagData = Tagger.ReadTags(file);
        if (
            tagData.AlbumArtist.SequenceEqual(album.AlbumArtist)
            && tagData.Album == album.AlbumName
            && tagData.Year == album.Year
        )
        {
            sb.AppendLine("Skipped");
            return await Task.FromResult(ProcessFileResult.Skipped(sb.ToString()));
        }
        tagData.AlbumArtist = album.AlbumArtist;
        tagData.Album = album.AlbumName;
        tagData.Year = album.Year;
        Tagger.WriteTags(file, tagData);
        return await Task.FromResult(ProcessFileResult.Success(sb.ToString()));
    }
}

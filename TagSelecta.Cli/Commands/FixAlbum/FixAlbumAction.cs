using Spectre.Console;
using TagSelecta.Cli.Commands.Common;
using TagSelecta.Shared;
using TagSelecta.Shared.Configuration;

namespace TagSelecta.Cli.Commands.FixAlbum;

public class FixAlbumAction(IAnsiConsole console) : TagDataAction<FixAlbumSettings>
{
    private readonly List<Album> _albums = [];

    protected override void ProcessTagData(
        FileWithTagData current,
        List<FileWithTagData> files,
        FixAlbumSettings settings,
        ILookup<string, string?> remainingOptions
    )
    {
        var dir = Directory.GetParent(current.Path)!.FullName;
        var album = _albums.SingleOrDefault(x => x.Dir == dir);
        if (album is null)
        {
            var dirTagData = files
                .Where(x => Directory.GetParent(x.Path)?.FullName == dir)
                .OrderBy(x => x.Path)
                .Select(x => x.TagData)
                .ToList();

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
            var albumDate = dirTagData
                .Select(x => x.Date)
                .Where(x => string.IsNullOrWhiteSpace(x))
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

            List<string> albumArtist;

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
                AlbumArtists = albumArtist,
                Date = albumDate,
            };
            _albums.Add(album);
        }
        var albumArtistMessage = album.FixType switch
        {
            FixType.PrimaryArtists =>
                $"Primary artist(s) identified: [yellow]{album.AlbumArtists.ToJoined().EscapeMarkup()}[/]",
            FixType.AllArtists =>
                $"No primary artist(s) detected. Using all contributing artists: [yellow]{album.AlbumArtists.ToJoined().EscapeMarkup()}[/]",
            FixType.VariousArtists =>
                $"Multiple distinct artists detected. Assigning album artist as: [yellow]{album.AlbumArtists.ToJoined().EscapeMarkup()}[/]",
            _ => "",
        };
        console.MarkupLine(albumArtistMessage);
        console.MarkupLine(
            $"The most common album mame: [yellow]{album.AlbumName.EscapeMarkup()}[/]"
        );
        console.MarkupLine($"The most common album year: [yellow]{album.Date}[/]");
        current.TagData.AlbumArtist = album.AlbumArtists;
        current.TagData.Album = album.AlbumName;
        current.TagData.Date = album.Date;
    }
}

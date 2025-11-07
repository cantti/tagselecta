using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class FixAlbumSettings : FileSettings { }

public class FixAlbumCommand(IAnsiConsole console) : FileCommand<FixAlbumSettings>(console)
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
        public required List<string> AlbumArtists { get; set; } = [];
        public required string AlbumName { get; set; }
        public required uint Year { get; set; }
    }

    private readonly List<Album> _albums = [];

    protected override void Execute(string file, int index)
    {
        var dir = Directory.GetParent(file)!.FullName;
        var album = _albums.SingleOrDefault(x => x.Dir == dir);
        if (album is null)
        {
            var filesInDir = Files
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
                .SelectMany(x => x.Artists)
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
                AlbumArtists = albumArtist,
                Year = albumYear,
            };
            _albums.Add(album);
        }
        string albumArtistMessage = album.FixType switch
        {
            FixType.PrimaryArtists =>
                $"Primary artist(s) identified: [yellow]{album.AlbumArtists.Joined().EscapeMarkup()}[/]",
            FixType.AllArtists =>
                $"No primary artist(s) detected. Using all contributing artists: [yellow]{album.AlbumArtists.Joined().EscapeMarkup()}[/]",
            FixType.VariousArtists =>
                $"Multiple distinct artists detected. Assigning album artist as: [yellow]{album.AlbumArtists.Joined().EscapeMarkup()}[/]",
            _ => "",
        };
        Console.MarkupLine(albumArtistMessage);
        Console.MarkupLine(
            $"The most common album mame: [yellow]{album.AlbumName.EscapeMarkup()}[/]"
        );
        Console.MarkupLine($"The most common album year: [yellow]{album.Year}[/]");
        var tagData = Tagger.ReadTags(file);
        if (
            tagData.AlbumArtists.SequenceEqual(album.AlbumArtists)
            && tagData.Album == album.AlbumName
            && tagData.Year == album.Year
        )
        {
            Console.MarkupLine("Skipped");
            Skip();
        }
        else
        {
            tagData.AlbumArtists = album.AlbumArtists;
            tagData.Album = album.AlbumName;
            tagData.Year = album.Year;
            if (ConfirmPrompt())
            {
                Tagger.WriteTags(file, tagData);
            }
        }
    }
}

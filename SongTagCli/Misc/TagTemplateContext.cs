using SongTagCli.Tagging;

namespace SongTagCli.Misc;

public class TagTemplateContext
{
    public TagTemplateContext(TagData tag, string path)
    {
        Path = path;
        ArtistList = tag.Artist;
        AlbumArtistList = tag.AlbumArtist;
        Album = tag.Album ?? "";
        Title = tag.Title ?? "";
        GenreList = tag.Genre;
        Year = tag.Year;
        Track = tag.Track;
        TrackTotal = tag.TrackTotal;
        Disc = tag.Disc;
        DiscTotal = tag.DiscTotal;
        Comment = tag.Comment ?? "";
        Label = tag.Label ?? "";
        CatalogNumber = tag.CatalogNumber ?? "";
    }

    public string Path { get; set; } = "";

    public List<string> ArtistList { get; set; } = [];

    public string Artist => ArtistList.Print();

    public List<string> AlbumArtistList { get; set; } = [];

    public string AlbumArtist => ArtistList.Print();

    public string Album { get; set; } = "";

    public string Title { get; set; } = "";

    public List<string> GenreList { get; set; } = [];

    public string Genre => GenreList.Print();

    public uint Year { get; set; }

    public uint Track { get; set; }

    public uint TrackTotal { get; set; }

    public uint Disc { get; set; }

    public uint DiscTotal { get; set; }

    public string Comment { get; set; } = "";

    public string Label { get; set; } = "";

    public string CatalogNumber { get; set; } = "";
}

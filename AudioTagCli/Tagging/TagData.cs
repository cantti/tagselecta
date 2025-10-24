namespace AudioTagCli.Tagging;

public class TagData
{
    public List<string> Artist { get; set; } = [];

    public List<string> AlbumArtist { get; set; } = [];

    public string AlbumArtistJoined => string.Join("; ", AlbumArtist);

    public string? Album { get; set; } = "";

    public string? Title { get; set; }

    public List<string> Genre { get; set; } = [];

    public uint Year { get; set; }

    public uint Track { get; set; }

    public uint TrackTotal { get; set; }

    public uint Disc { get; set; }

    public uint DiscTotal { get; set; }

    public string? Comments { get; set; }

    public string? Label { get; set; }

    public string? CatalogNumber { get; set; }

    public List<TagLib.IPicture> Pictures { get; set; } = [];
    public string Path { get; internal set; }
}

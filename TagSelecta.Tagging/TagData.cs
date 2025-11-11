using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagData
{
    public string Path { get; set; } = "";

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    [Printable("Album Artist")]
    [XiphKey("albumartist", "album artist", "ensemble")]
    public List<string> AlbumArtists { get; set; } = [];

    public string AlbumArtist => AlbumArtists.Joined();

    [Printable("Artist")]
    [XiphKey("artist")]
    public List<string> Artists { get; set; } = [];

    public string Artist => Artists.Joined();

    [Printable]
    [XiphKey("album")]
    public string Album { get; set; } = "";

    [Printable]
    [XiphKey("date")]
    public uint Year { get; set; }

    [Printable]
    [XiphKey("title")]
    public string Title { get; set; } = "";

    [Printable]
    [XiphKey("tracknumber")]
    public uint Track { get; set; }

    [Printable("Track Total")]
    [XiphKey("tracktotal")]
    public uint TrackTotal { get; set; }

    [Printable]
    [XiphKey("disc")]
    public uint Disc { get; set; }

    [Printable("Disc Total")]
    [XiphKey("disctotal")]
    public uint DiscTotal { get; set; }

    [Printable("Genre")]
    [XiphKey("genre")]
    public List<string> Genres { get; set; } = [];

    public string Genre => Genres.Joined();

    [Printable]
    [XiphKey("comment")]
    public string Comment { get; set; } = "";

    [Printable]
    [XiphKey("composer")]
    public List<string> Composers { get; set; } = [];

    public string Composer => Composers.Joined();

    public List<CustomTag> Custom { get; set; } = [];

    [Printable]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}

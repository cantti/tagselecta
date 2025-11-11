using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagData
{
    public string Path { get; set; } = "";

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    [Editable("Album Artist")]
    [XiphKey("albumartist", "album artist", "ensemble")]
    public List<string> AlbumArtists { get; set; } = [];

    public string AlbumArtist => AlbumArtists.Joined();

    [Editable("Artist")]
    [XiphKey("artist")]
    public List<string> Artists { get; set; } = [];

    public string Artist => Artists.Joined();

    [Editable]
    [XiphKey("album")]
    public string Album { get; set; } = "";

    [Editable]
    [XiphKey("date")]
    public uint Year { get; set; }

    [Editable]
    [XiphKey("title")]
    public string Title { get; set; } = "";

    [Editable]
    [XiphKey("tracknumber")]
    public uint Track { get; set; }

    [Editable("Track Total")]
    [XiphKey("tracktotal")]
    public uint TrackTotal { get; set; }

    [Editable]
    [XiphKey("disc")]
    public uint Disc { get; set; }

    [Editable("Disc Total")]
    [XiphKey("disctotal")]
    public uint DiscTotal { get; set; }

    [Editable("Genre")]
    [XiphKey("genre")]
    public List<string> Genres { get; set; } = [];

    public string Genre => Genres.Joined();

    [Editable]
    [XiphKey("comment")]
    public string Comment { get; set; } = "";

    [Editable]
    [XiphKey("composer")]
    public List<string> Composers { get; set; } = [];

    public string Composer => Composers.Joined();

    public Dictionary<string, string> Custom { get; set; } = [];

    [Editable]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}

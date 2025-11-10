using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagData
{
    public string Path { get; set; } = "";

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    [Editable("Album Artist")]
    public List<string> AlbumArtists { get; set; } = [];

    public string AlbumArtist => AlbumArtists.Joined();

    [Editable("Artist")]
    public List<string> Artists { get; set; } = [];

    public string Artist => Artists.Joined();

    [Editable]
    public string Album { get; set; } = "";

    [Editable]
    public uint Year { get; set; }

    [Editable]
    public string Title { get; set; } = "";

    [Editable]
    public string Subtitle { get; set; } = "";

    [Editable]
    public uint Track { get; set; }

    [Editable("Track Total")]
    public uint TrackTotal { get; set; }

    [Editable]
    public uint Disc { get; set; }

    [Editable("Disc Total")]
    public uint DiscTotal { get; set; }

    [Editable("Genre")]
    public List<string> Genres { get; set; } = [];

    public string Genre => Genres.Joined();

    [Editable]
    public string Comment { get; set; } = "";

    [Editable]
    public string Description { get; set; } = "";

    [Editable]
    public string Label { get; set; } = "";

    [Editable("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    [Editable]
    public uint Bpm { get; set; }

    [Editable]
    public List<string> Composers { get; set; } = [];

    public string Composer => Composers.Joined();

    [Editable]
    public string Conductor { get; set; } = "";

    [Editable]
    public string Isrc { get; set; } = "";

    [Editable]
    public string Lyrics { get; set; } = "";

    [Editable]
    public string Publisher { get; set; } = "";

    [Editable("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    [Editable("MusicBrainz Artist Id")]
    public string MusicBrainzArtistId { get; set; } = "";

    [Editable("MusicBrainz Disc Id")]
    public string MusicBrainzDiscId { get; set; } = "";

    [Editable("MusicBrainz Release Group Id")]
    public string MusicBrainzReleaseGroupId { get; set; } = "";

    [Editable("MusicBrainz Release Id")]
    public string MusicBrainzReleaseId { get; set; } = "";

    [Editable("MusicBrainz Release Artist Id")]
    public string MusicBrainzReleaseArtistId { get; set; } = "";

    [Editable("MusicBrainz Track Id")]
    public string MusicBrainzTrackId { get; set; } = "";

    [Editable("MusicBrainz Release Status")]
    public string MusicBrainzReleaseStatus { get; set; } = "";

    [Editable("MusicBrainz Release Type")]
    public string MusicBrainzReleaseType { get; set; } = "";

    [Editable("MusicBrainz Release Country")]
    public string MusicBrainzReleaseCountry { get; set; } = "";

    [Editable("Replay Gain Track Gain")]
    public double? ReplayGainTrackGain { get; set; }

    [Editable("Replay Gain Track Peak")]
    public double? ReplayGainTrackPeak { get; set; }

    [Editable("Replay Gain Album Gain")]
    public double? ReplayGainAlbumGain { get; set; }

    [Editable("Replay Gain Album Peak")]
    public double? ReplayGainAlbumPeak { get; set; }

    [Editable]
    public string Copyright { get; set; } = "";

    [Editable]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}

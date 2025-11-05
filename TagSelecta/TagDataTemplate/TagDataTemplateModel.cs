namespace TagSelecta.TagDataTemplate;

public class TagDataTemplateModel
{
    public string Path { get; set; } = "";

    public string Filename { get; set; } = "";

    public string Artist { get; set; } = "";

    public List<string> ArtistList { get; set; } = [];

    public string AlbumArtist { get; set; } = "";

    public List<string> AlbumArtistList { get; set; } = [];

    public string Album { get; set; } = "";

    public string Title { get; set; } = "";

    public string Subtitle { get; set; } = "";

    public string Genre { get; set; } = "";

    public List<string> GenreList { get; set; } = [];

    public uint Year { get; set; }

    public uint Track { get; set; }

    public uint TrackTotal { get; set; }

    public uint Disc { get; set; }

    public uint DiscTotal { get; set; }

    public string Comment { get; set; } = "";

    public string Description { get; set; } = "";

    public string Label { get; set; } = "";

    public string CatalogNumber { get; set; } = "";

    public uint Bpm { get; set; }

    public string Composers { get; set; } = "";

    public List<string> ComposerList { get; set; } = [];

    public string Conductor { get; set; } = "";

    public string Isrc { get; set; } = "";

    public string Lyrics { get; set; } = "";

    public string Publisher { get; set; } = "";

    public string AmazonId { get; set; } = "";

    public string MusicBrainzArtistId { get; set; } = "";

    public string MusicBrainzDiscId { get; set; } = "";

    public string MusicBrainzReleaseGroupId { get; set; } = "";

    public string MusicBrainzReleaseId { get; set; } = "";

    public string MusicBrainzReleaseArtistId { get; set; } = "";

    public string MusicBrainzTrackId { get; set; } = "";

    public string MusicBrainzReleaseStatus { get; set; } = "";

    public string MusicBrainzReleaseType { get; set; } = "";

    public string MusicBrainzReleaseCountry { get; set; } = "";

    public string MusicIpId { get; set; } = "";

    public double? ReplayGainTrackGain { get; set; }

    public double? ReplayGainTrackPeak { get; set; }

    public double? ReplayGainAlbumGain { get; set; }

    public double? ReplayGainAlbumPeak { get; set; }

    public string Copyright { get; set; } = "";

    public string DiscogsReleaseId { get; set; } = "";
}

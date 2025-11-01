using TagLib;

namespace TagSelecta.Print;

public class TagDataForJson
{
    [SkipNoValue]
    public List<string> Artist { get; set; } = [];

    [SkipNoValue]
    public List<string> AlbumArtist { get; set; } = [];

    [SkipNoValue]
    public List<string> Composers { get; set; } = [];

    [SkipNoValue]
    public string Album { get; set; } = "";

    [SkipNoValue]
    public string Title { get; set; } = "";

    [SkipNoValue]
    public string Subtitle { get; set; } = "";

    [SkipNoValue]
    public List<string> Genre { get; set; } = [];

    [SkipNoValue]
    public uint Year { get; set; }

    [SkipNoValue]
    public uint Track { get; set; }

    [SkipNoValue]
    public uint TrackTotal { get; set; }

    [SkipNoValue]
    public uint Disc { get; set; }

    [SkipNoValue]
    public uint DiscTotal { get; set; }

    [SkipNoValue]
    public string Comment { get; set; } = "";

    [SkipNoValue]
    public string Description { get; set; } = "";

    [SkipNoValue]
    public string Label { get; set; } = "";

    [SkipNoValue]
    public string CatalogNumber { get; set; } = "";

    [SkipNoValue]
    public uint Bpm { get; set; }

    [SkipNoValue]
    public string Conductor { get; set; } = "";

    [SkipNoValue]
    public string Isrc { get; set; } = "";

    [SkipNoValue]
    public string Lyrics { get; set; } = "";

    [SkipNoValue]
    public string Publisher { get; set; } = "";

    [SkipNoValue]
    public string AmazonId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzArtistId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzDiscId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzReleaseGroupId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzReleaseId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzReleaseArtistId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzTrackId { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzReleaseStatus { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzReleaseType { get; set; } = "";

    [SkipNoValue]
    public string MusicBrainzReleaseCountry { get; set; } = "";

    [SkipNoValue]
    public string MusicIpId { get; set; } = "";

    [SkipNoValue]
    public double? ReplayGainTrackGain { get; set; }

    [SkipNoValue]
    public double? ReplayGainTrackPeak { get; set; }

    [SkipNoValue]
    public double? ReplayGainAlbumGain { get; set; }

    [SkipNoValue]
    public double? ReplayGainAlbumPeak { get; set; }

    [SkipNoValue]
    public string Copyright { get; set; } = "";

    [SkipNoValue]
    public List<PictureDataForJson> Pictures { get; set; } = [];
}

public class PictureDataForJson
{
    [SkipNoValue]
    public string MimeType { get; set; } = "";

    [SkipNoValue]
    public string Type { get; set; } = "";

    [SkipNoValue]
    public string Filename { get; set; } = "";

    [SkipNoValue]
    public string Description { get; set; } = "";
}

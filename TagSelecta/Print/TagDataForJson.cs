using System.ComponentModel;
using System.Text.Json.Serialization;
using TagLib;
using TagSelecta.Tagging;

namespace TagSelecta.Print;

public class TagDataForJson(TagData t)
{
    public List<string> Artist => t.Artist;

    public List<string> AlbumArtist => t.AlbumArtist;

    public string Album => t.Album;

    public string Title => t.Title;

    public List<string> Genre => t.Genre;

    public uint Year => t.Year;

    public uint Track => t.Track;

    public uint TrackTotal => t.TrackTotal;

    [SkipEmptyOrZero]
    public uint Disc => t.Disc;

    [SkipEmptyOrZero]
    public uint DiscTotal => t.DiscTotal;

    [SkipEmptyOrZero]
    public string Comment => t.Comment;

    [SkipEmptyOrZero]
    public string Label => t.Label;

    [SkipEmptyOrZero]
    public string CatalogNumber => t.CatalogNumber;

    [SkipEmptyOrZero]
    public uint Bpm => t.Bpm;

    public List<PictureDataForJson> Pictures =>
        [.. t.Pictures.Select(x => new PictureDataForJson(x))];
}

public class PictureDataForJson(IPicture pic)
{
    public string MimeType => pic.MimeType ?? "";
    public string Type => pic.Type.ToString();
    public string Filename => pic.Filename ?? "";
    public string Description => pic.Description ?? "";
}

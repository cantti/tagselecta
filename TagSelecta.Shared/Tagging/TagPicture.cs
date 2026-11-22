using TagLib;

namespace TagSelecta.Shared.Tagging;

public sealed class TagPicture
{
    private readonly byte[] _data;

    public TagPicture(IPicture picture)
    {
        Type = picture.Type;
        Filename = picture.Filename;
        MimeType = picture.MimeType;
        Description = picture.Description;
        _data = picture.Data.ToArray();
    }

    public PictureType Type { get; }

    public string Filename { get; }

    public string MimeType { get; }

    public string Description { get; }

    public ReadOnlySpan<byte> Data => _data;

    public Picture ToTagLibPicture()
    {
        return new Picture
        {
            Type = Type,
            Filename = Filename,
            MimeType = MimeType,
            Description = Description,
            Data = new ByteVector(_data.ToArray()),
        };
    }

    public string GetExtension()
    {
        return Picture.GetExtensionFromData(new ByteVector(_data.ToArray()));
    }
}

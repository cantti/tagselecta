using TagLib;

namespace TagSelecta.Shared.Tagging;

public sealed class TagPicture
{
    private readonly IReadOnlyList<byte> _data;

    public TagPicture(IPicture picture)
    {
        Type = picture.Type;
        Filename = picture.Filename;
        MimeType = picture.MimeType;
        Description = picture.Description;
        _data = Array.AsReadOnly(picture.Data.ToArray());
    }

    public PictureType Type { get; }

    public string Filename { get; }

    public string MimeType { get; }

    public string Description { get; }

    public IReadOnlyList<byte> Data => _data;

    public Picture ToTagLibPicture()
    {
        return new Picture
        {
            Type = Type,
            Filename = Filename,
            MimeType = MimeType,
            Description = Description,
            Data = [.. _data.ToArray()],
        };
    }

    public string GetExtension()
    {
        return Picture.GetExtensionFromData([.. _data.ToArray()]);
    }
}

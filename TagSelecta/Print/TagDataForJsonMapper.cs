using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using TagSelecta.Tagging;

namespace TagSelecta.Print;

[Mapper]
public static partial class TagDataForJsonMapper
{
    public static partial TagDataForJson Map(TagData tag);

    [SuppressMessage("Mapper", "RMG089")]
    [SuppressMessage("Mapper", "RMG090")]
    [MapperIgnoreSource(nameof(TagLib.Picture.Data))]
    private static partial PictureDataForJson PictureMap(TagLib.Picture pic);
}

using Riok.Mapperly.Abstractions;
using TagSelecta.Tagging;

namespace TagSelecta.Print;

[Mapper]
public static partial class TagDataForJsonMapper
{
    public static partial TagDataForJson Map(TagData tag);
}

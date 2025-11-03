using Riok.Mapperly.Abstractions;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.TagTemplate;

[Mapper]
public partial class TagTemplateContextMapper
{
    [MapperIgnoreSource(nameof(TagData.Picture))]
    [MapProperty(nameof(TagData.Artist), nameof(TagTemplateContext.ArtistList))]
    [MapProperty(nameof(TagData.AlbumArtist), nameof(TagTemplateContext.AlbumArtistList))]
    [MapProperty(nameof(TagData.Genre), nameof(TagTemplateContext.GenreList))]
    [MapProperty(nameof(TagData.Composers), nameof(TagTemplateContext.ComposerList))]
    public static partial TagTemplateContext Map(TagData tag);

    public static string ListToString(List<string> list) => list.Print();
}

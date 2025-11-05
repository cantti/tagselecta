using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataTemplate;

[Mapper]
public static partial class TagDataTemplateModelMapper
{
    [MapperIgnoreSource(nameof(TagData.Picture))]
    [MapperIgnoreTarget(nameof(TagDataTemplateModel.Path))]
    [MapperIgnoreTarget(nameof(TagDataTemplateModel.Filename))]
    [MapProperty(nameof(TagData.Artist), nameof(TagDataTemplateModel.ArtistList))]
    [MapProperty(nameof(TagData.AlbumArtist), nameof(TagDataTemplateModel.AlbumArtistList))]
    [MapProperty(nameof(TagData.Genre), nameof(TagDataTemplateModel.GenreList))]
    [MapProperty(nameof(TagData.Composers), nameof(TagDataTemplateModel.ComposerList))]
    public static partial TagDataTemplateModel Map(TagData tag);

    [SuppressMessage("Mapper", "IDE0051")] // why???
    private static string MapListToString(List<string> list) => list.Print();
}

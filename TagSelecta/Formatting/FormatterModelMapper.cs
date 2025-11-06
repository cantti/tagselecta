using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Formatting;

[Mapper]
public static partial class FormatterModelMapper
{
    [MapperIgnoreSource(nameof(TagData.Picture))]
    [MapperIgnoreTarget(nameof(FormatterModel.Path))]
    [MapperIgnoreTarget(nameof(FormatterModel.Filename))]
    [MapProperty(nameof(TagData.Artist), nameof(FormatterModel.ArtistList))]
    [MapProperty(nameof(TagData.AlbumArtist), nameof(FormatterModel.AlbumArtistList))]
    [MapProperty(nameof(TagData.Genre), nameof(FormatterModel.GenreList))]
    [MapProperty(nameof(TagData.Composers), nameof(FormatterModel.ComposerList))]
    public static partial FormatterModel Map(TagData tag);

    [SuppressMessage("Mapper", "IDE0051")] // why???
    private static string MapListToString(List<string> list) => list.Print();
}

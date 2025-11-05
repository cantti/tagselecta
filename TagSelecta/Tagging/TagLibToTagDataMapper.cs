using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using TagLib;

namespace TagSelecta.Tagging;

[Mapper(
    PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive,
    IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both
)]
public static partial class TagLibToTagDataMapper
{
    [SuppressMessage("Mapper", "RMG089")]
    [SuppressMessage("Mapper", "RMG090")]
    // custom mapping
    [MapProperty(nameof(Tag.AlbumArtists), nameof(TagData.AlbumArtist))]
    [MapProperty(nameof(Tag.Performers), nameof(TagData.Artist))]
    [MapProperty(nameof(Tag.TrackCount), nameof(TagData.TrackTotal))]
    [MapProperty(nameof(Tag.DiscCount), nameof(TagData.DiscTotal))]
    [MapProperty(nameof(Tag.Genres), nameof(TagData.Genre))]
    [MapProperty(nameof(Tag.BeatsPerMinute), nameof(TagData.Bpm))]
    [MapProperty(nameof(Tag.Pictures), nameof(TagData.Picture))]
    // ignore source
    [MapperIgnoreSource(nameof(Tag.IsEmpty))]
    [MapperIgnoreSource(nameof(Tag.Length))]
    [MapperIgnoreSource(nameof(Tag.InitialKey))]
    [MapperIgnoreSource(nameof(Tag.DateTagged))]
    [MapperIgnoreSource(nameof(Tag.TagTypes))]
    [MapperIgnoreSource(nameof(Tag.TitleSort))]
    [MapperIgnoreSource(nameof(Tag.PerformersSort))]
    [MapperIgnoreSource(nameof(Tag.PerformersRole))]
    [MapperIgnoreSource(nameof(Tag.AlbumArtistsSort))]
    [MapperIgnoreSource(nameof(Tag.AlbumSort))]
    [MapperIgnoreSource(nameof(Tag.ComposersSort))]
    [MapperIgnoreSource(nameof(Tag.Grouping))]
    [MapperIgnoreSource(nameof(Tag.FirstAlbumArtist))]
    [MapperIgnoreSource(nameof(Tag.FirstAlbumArtistSort))]
    [MapperIgnoreSource(nameof(Tag.FirstComposer))]
    [MapperIgnoreSource(nameof(Tag.FirstPerformerSort))]
    [MapperIgnoreSource(nameof(Tag.FirstComposerSort))]
    [MapperIgnoreSource(nameof(Tag.FirstGenre))]
    [MapperIgnoreSource(nameof(Tag.FirstPerformer))]
    [MapperIgnoreSource(nameof(Tag.JoinedAlbumArtists))]
    [MapperIgnoreSource(nameof(Tag.JoinedComposers))]
    [MapperIgnoreSource(nameof(Tag.JoinedGenres))]
    [MapperIgnoreSource(nameof(Tag.JoinedPerformers))]
    [MapperIgnoreSource(nameof(Tag.JoinedPerformersSort))]
    [MapperIgnoreSource(nameof(Tag.RemixedBy))]
    // ignore target
    [MapperIgnoreTarget(nameof(TagData.Label))]
    [MapperIgnoreTarget(nameof(TagData.CatalogNumber))]
    [MapperIgnoreTarget(nameof(TagData.DiscogsReleaseId))]
    public static partial TagData Map(Tag tag);

    // taglib stores no value as nan for double
    [SuppressMessage("Mapper", "IDE0051")] // why???
    private static double? MapDouble(double source) => double.IsNaN(source) ? null : source;
}

using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using TagLib;

namespace TagSelecta.Tagging;

[Mapper(
    PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive,
    IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both
)]
public static partial class TagDataToTagLibMapper
{
    [SuppressMessage("Mapper", "RMG089")]
    [SuppressMessage("Mapper", "RMG090")]
    // custom mapping
    [MapProperty(nameof(TagData.Artists), nameof(Tag.Performers))]
    [MapProperty(nameof(TagData.TrackTotal), nameof(Tag.TrackCount))]
    [MapProperty(nameof(TagData.DiscTotal), nameof(Tag.DiscCount))]
    [MapProperty(nameof(TagData.Bpm), nameof(Tag.BeatsPerMinute))]
    [MapProperty(nameof(TagData.Pictures), nameof(Tag.Pictures))]
    // ignore source
    [MapperIgnoreTarget(nameof(Tag.IsEmpty))]
    [MapperIgnoreTarget(nameof(Tag.Length))]
    [MapperIgnoreTarget(nameof(Tag.InitialKey))]
    [MapperIgnoreTarget(nameof(Tag.DateTagged))]
    [MapperIgnoreTarget(nameof(Tag.TagTypes))]
    [MapperIgnoreTarget(nameof(Tag.TitleSort))]
    [MapperIgnoreTarget(nameof(Tag.PerformersSort))]
    [MapperIgnoreTarget(nameof(Tag.PerformersRole))]
    [MapperIgnoreTarget(nameof(Tag.AlbumArtistsSort))]
    [MapperIgnoreTarget(nameof(Tag.AlbumSort))]
    [MapperIgnoreTarget(nameof(Tag.ComposersSort))]
    [MapperIgnoreTarget(nameof(Tag.Grouping))]
    [MapperIgnoreTarget(nameof(Tag.FirstAlbumArtist))]
    [MapperIgnoreTarget(nameof(Tag.FirstAlbumArtistSort))]
    [MapperIgnoreTarget(nameof(Tag.FirstComposer))]
    [MapperIgnoreTarget(nameof(Tag.FirstPerformerSort))]
    [MapperIgnoreTarget(nameof(Tag.FirstComposerSort))]
    [MapperIgnoreTarget(nameof(Tag.FirstGenre))]
    [MapperIgnoreTarget(nameof(Tag.FirstPerformer))]
    [MapperIgnoreTarget(nameof(Tag.JoinedAlbumArtists))]
    [MapperIgnoreTarget(nameof(Tag.JoinedComposers))]
    [MapperIgnoreTarget(nameof(Tag.JoinedGenres))]
    [MapperIgnoreTarget(nameof(Tag.JoinedPerformers))]
    [MapperIgnoreTarget(nameof(Tag.JoinedPerformersSort))]
    [MapperIgnoreTarget(nameof(Tag.RemixedBy))]
    // ignore source
    [MapperIgnoreSource(nameof(TagData.Label))]
    [MapperIgnoreSource(nameof(TagData.CatalogNumber))]
    [MapperIgnoreSource(nameof(TagData.DiscogsReleaseId))]
    [MapperIgnoreSource(nameof(TagData.Path))]
    [MapperIgnoreSource(nameof(TagData.FileName))]
    [MapperIgnoreSource(nameof(TagData.Artist))]
    [MapperIgnoreSource(nameof(TagData.AlbumArtist))]
    [MapperIgnoreSource(nameof(TagData.Genre))]
    [MapperIgnoreSource(nameof(TagData.Composer))]
    public static partial void Map(TagData tagData, Tag tag);

    // taglib stores no value as nan for double
    [SuppressMessage("Mapper", "IDE0051")] // why???
    private static double MapDouble(double? source) => source is null ? double.NaN : source.Value;
}

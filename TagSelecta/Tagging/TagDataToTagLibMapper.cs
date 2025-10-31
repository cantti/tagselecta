using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using TagLib;

namespace TagSelecta.Tagging;

[Mapper(
    PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive,
    IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both
)]
public partial class TagDataToTagLibMapper
{
    [SuppressMessage("Mapper", "RMG089")]
    [SuppressMessage("Mapper", "RMG090")]
    // custom mapping
    [MapProperty(nameof(TagData.AlbumArtist), nameof(Tag.AlbumArtists))]
    [MapProperty(nameof(TagData.Artist), nameof(Tag.Performers))]
    [MapProperty(nameof(TagData.TrackTotal), nameof(Tag.TrackCount))]
    [MapProperty(nameof(TagData.DiscTotal), nameof(Tag.DiscCount))]
    [MapProperty(nameof(TagData.Genre), nameof(Tag.Genres))]
    [MapProperty(nameof(TagData.Bpm), nameof(Tag.BeatsPerMinute))]
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
    // ignore target
    [MapperIgnoreSource(nameof(TagData.Label))]
    [MapperIgnoreSource(nameof(TagData.CatalogNumber))]
    public partial void Map(TagData tagData, Tag tag);
}

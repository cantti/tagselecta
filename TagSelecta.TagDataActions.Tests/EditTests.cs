using NSubstitute;
using TagSelecta.Shared.Http;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Edit;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class EditTests
{
    [Fact]
    public async Task EditTest()
    {
        // Arrange
        var downloader = Substitute.For<IDownloader>();

        ITagDataAction action = new EditAction(downloader);

        var settings = new EditSettings
        {
            Album = "Test Album",
            AlbumArtist = "Test Album Artist",
            Artist = "Test Artist",
            Bpm = "120",
            Comment = "Test comment",
            Composer = "Test Composer",
            Conductor = "Test Conductor",
            Copyright = "Test Copyright",
            Date = "2025",
            DiscNumber = "1",
            DiscTotal = "2",
            Genre = "Test Genre",
            Isrc = "TESTISRC123",
            Publisher = "Test Publisher",
            Title = "Test Title",
            TrackNumber = "5",
            TrackTotal = "12",
            Key = ["test_field"],
            Value = ["test_value"],
        };

        var tagData = new TagData { Picture = [] };
        tagData.SetValue(FieldName.Album, "Original Album");
        tagData.SetValue(FieldName.AlbumArtist, ["Original Album Artist"]);
        tagData.SetValue(FieldName.Artist, ["Original Artist"]);
        tagData.SetValue(FieldName.Bpm, "90");
        tagData.SetValue(FieldName.Comment, "Original comment");
        tagData.SetValue(FieldName.Composer, ["Original Composer"]);
        tagData.SetValue(FieldName.Conductor, "Original Conductor");
        tagData.SetValue(FieldName.Copyright, "Original Copyright");
        tagData.SetValue(FieldName.Date, "2000");
        tagData.SetValue(FieldName.DiscNumber, "1");
        tagData.SetValue(FieldName.DiscTotal, "1");
        tagData.SetValue(FieldName.Genre, ["Original Genre"]);
        tagData.SetValue(FieldName.Isrc, "ORIGINALISRC");
        tagData.SetValue(FieldName.Publisher, "Original Publisher");
        tagData.SetValue(FieldName.Title, "Original Title");
        tagData.SetValue(FieldName.TrackNumber, "1");
        tagData.SetValue(FieldName.TrackTotal, "10");
        tagData.SetValue("label", "Original Label");
        tagData.SetValue("catalognumber", "Original Catalog Number");

        var item = new TestTarget("file.mp3", tagData);

        // Act
        await action.BeforeExecute(settings, CancellationToken.None);
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item },
            CancellationToken.None
        );

        // Assert
        var currentTagData = item.CurrentTagData;
        Assert.Equal([settings.Album], currentTagData.GetValue(FieldName.Album));
        Assert.Equal([settings.AlbumArtist], currentTagData.GetValue(FieldName.AlbumArtist));
        Assert.Equal(settings.Artist!.SplitTagValues(), currentTagData.GetValue(FieldName.Artist));
        Assert.Equal([settings.Bpm], currentTagData.GetValue(FieldName.Bpm));
        Assert.Equal([settings.Comment], currentTagData.GetValue(FieldName.Comment));
        Assert.Equal([settings.Composer], currentTagData.GetValue(FieldName.Composer));
        Assert.Equal([settings.Conductor], currentTagData.GetValue(FieldName.Conductor));
        Assert.Equal([settings.Copyright], currentTagData.GetValue(FieldName.Copyright));
        Assert.Equal([settings.Date], currentTagData.GetValue(FieldName.Date));
        Assert.Equal([settings.DiscNumber], currentTagData.GetValue(FieldName.DiscNumber));
        Assert.Equal([settings.DiscTotal], currentTagData.GetValue(FieldName.DiscTotal));
        Assert.Equal([settings.Genre], currentTagData.GetValue(FieldName.Genre));
        Assert.Equal([settings.Isrc], currentTagData.GetValue(FieldName.Isrc));
        Assert.Equal([settings.Publisher], currentTagData.GetValue(FieldName.Publisher));
        Assert.Equal([settings.Title], currentTagData.GetValue(FieldName.Title));
        Assert.Equal([settings.TrackNumber], currentTagData.GetValue(FieldName.TrackNumber));
        Assert.Equal([settings.TrackTotal], currentTagData.GetValue(FieldName.TrackTotal));
        Assert.Equal([settings.Value[0]], currentTagData.GetValue(settings.Key[0]));
        Assert.Equal(["Original Label"], currentTagData.GetValue("label"));
        Assert.Equal(["Original Catalog Number"], currentTagData.GetValue("catalognumber"));
    }

    [Fact]
    public void EditSettingsContainsAllFieldNames()
    {
        var properties = typeof(EditSettings).GetProperties();
        var propertyNames = new HashSet<string>(
            properties.Select(x => x.Name),
            StringComparer.OrdinalIgnoreCase
        );

        var missing = FieldName.All().Where(field => !propertyNames.Contains(field)).ToList();

        Assert.True(
            missing.Count == 0,
            $"EditSettings is missing fields: {string.Join(", ", missing)}"
        );
    }
}

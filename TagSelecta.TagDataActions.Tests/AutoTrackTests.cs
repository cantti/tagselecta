using NSubstitute;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.AutoTrack;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class AutoTrackTests
{
    [Fact]
    public async Task AutoTrackTest()
    {
        // Arrange
        var audioFileScanner = Substitute.For<IAudioFileScanner>();
        audioFileScanner.Search(new List<string>()).ReturnsForAnyArgs(["file1.mp3", "file2.mp3"]);

        ITagDataAction action = new AutoTrackAction(audioFileScanner);

        var settings = new AutoTrackSettings { KeepDisk = true };

        var item1 = new TestTarget("file1.mp3", new TagData());
        item1.CurrentTagData.SetValue(FieldName.Disc, "1");
        item1.CurrentTagData.SetValue(FieldName.DiscTotal, "1");

        var item2 = new TestTarget("file2.mp3", new TagData());
        item2.CurrentTagData.SetValue(FieldName.Disc, "1");
        item2.CurrentTagData.SetValue(FieldName.DiscTotal, "1");

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item1 },
            CancellationToken.None
        );
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item2 },
            CancellationToken.None
        );

        // Assert
        var currentTagData1 = item1.CurrentTagData;
        var currentTagData2 = item2.CurrentTagData;
        Assert.Equal("1", currentTagData1.GetValueFirst(FieldName.Track));
        Assert.Equal("2", currentTagData1.GetValueFirst(FieldName.TrackTotal));
        Assert.Equal("1", currentTagData1.GetValueFirst(FieldName.Disc));
        Assert.Equal("1", currentTagData1.GetValueFirst(FieldName.DiscTotal));
        Assert.Equal("2", currentTagData2.GetValueFirst(FieldName.Track));
        Assert.Equal("2", currentTagData2.GetValueFirst(FieldName.TrackTotal));
        Assert.Equal("1", currentTagData2.GetValueFirst(FieldName.Disc));
        Assert.Equal("1", currentTagData2.GetValueFirst(FieldName.DiscTotal));
    }
}

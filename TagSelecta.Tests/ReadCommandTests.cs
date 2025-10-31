using TagSelecta.Actions;
using TagSelecta.BaseCommands;

namespace TagSelecta.Tests;

[Collection("Console")]
public class ReadCommandTests
{
    [Fact]
    public void ReadTest()
    {
        // Given
        var app = CommandAppFactory.CreateTestApp<FileCommand<ReadSettings>>();

        // When
        var result = app.Run("./TestData/ReadTest/01 Song 1.mp3");

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches(@"Artist\W+Test Artist", result.Output);
        Assert.Matches(@"AlbumArtist\W+Test Album Artist", result.Output);
        Assert.Matches(@"Title\W+Song 1", result.Output);
        Assert.Matches(@"Album\W+Test Album", result.Output);
        Assert.Matches(@"Genre\W+Rock\W+Pop", result.Output);
        Assert.Matches(@"Year\W+1990", result.Output);
        Assert.Matches(@"Track\W+1", result.Output);
        Assert.Matches(@"TrackTotal\W+3", result.Output);
        Assert.Matches(@"Disc\W+1", result.Output);
        Assert.Matches(@"DiscTotal\W+1", result.Output);
    }
}

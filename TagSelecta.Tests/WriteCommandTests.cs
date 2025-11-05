using Spectre.Console.Testing;
using TagSelecta.Commands;

namespace TagSelecta.Tests;

[Collection("Console")]
public class WriteCommandTests
{
    [Fact]
    public void WriteTest()
    {
        // Given
        var app = CommandAppFactory.CreateTestApp<WriteCommand>();
        app.Console.Interactive();
        app.Console.Input.PushTextWithEnter("y");

        // When
        var result = app.Run(
            "./TestData/WriteTest/01 Song 1.mp3",
            "-a",
            "New Test Artist",
            "-A",
            "New Test Album Artist",
            "-t",
            "New Song 1",
            "-l",
            "New Test Album",
            "-y",
            "2000",
            "-g",
            "Reggae",
            "-g",
            "Dub",
            "-n",
            "10",
            "-N",
            "20",
            "-d",
            "30",
            "-D",
            "40"
        );

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches("Artist.+New Test Artist", result.Output);
        Assert.Matches("AlbumArtist.+New Test Album Artist", result.Output);
        Assert.Matches("Title.+New Song 1", result.Output);
        Assert.Matches("Album.+New Test Album", result.Output);
        Assert.Matches("Genre.+Reggae.+\n.+Dub", result.Output);
        Assert.Matches("Year.+2000", result.Output);
        Assert.Matches("Track.+10", result.Output);
        Assert.Matches("TrackTotal.+20", result.Output);
        Assert.Matches("Disc.+30", result.Output);
        Assert.Matches("DiscTotal.+40", result.Output);
    }
}

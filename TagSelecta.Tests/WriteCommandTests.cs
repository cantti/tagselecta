using TagSelecta.Actions;
using TagSelecta.BaseCommands;

namespace TagSelecta.Tests;

[Collection("Console")]
public class WriteCommandTests
{
    [Fact]
    public void WriteTest()
    {
        // Given
        var app = CommandAppFactory.CreateTestApp<FileCommand<WriteSettings>>();

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
        Assert.Matches(@"Artist\W+New Test Artist", result.Output);
        Assert.Matches(@"Album Artist\W+New Test Album Artist", result.Output);
        Assert.Matches(@"Title\W+New Song 1", result.Output);
        Assert.Matches(@"Album\W+New Test Album", result.Output);
        Assert.Matches(@"Genre\W+Reggae\W+Dub", result.Output);
        Assert.Matches(@"Year\W+2000", result.Output);
        Assert.Matches(@"Track\W+10", result.Output);
        Assert.Matches(@"Track Total\W+20", result.Output);
        Assert.Matches(@"Disc\W+30", result.Output);
        Assert.Matches(@"Disc Total\W+40", result.Output);
    }
}

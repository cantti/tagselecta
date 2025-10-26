using SongTagCli.Commands;
using Spectre.Console.Testing;

namespace SongTagCli.Tests;

[Collection("Console")]
public class ReadCommandTests
{
    [Fact]
    public void ReadTest()
    {
        // Given
        var app = new CommandAppTester();
        app.SetDefaultCommand<ReadCommand>();

        // When
        var result = app.Run("./TestData/ReadTest/01 Song 1.mp3");

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches(@"Artist\W+Test Artist", result.Output);
        Assert.Matches(@"Album Artist\W+Test Album Artist", result.Output);
        Assert.Matches(@"Title\W+Song 1", result.Output);
        Assert.Matches(@"Album\W+Test Album", result.Output);
        Assert.Matches(@"Genre\W+Rock\W+Pop", result.Output);
        Assert.Matches(@"Year\W+1990", result.Output);
        Assert.Matches(@"Track\W+1", result.Output);
        Assert.Matches(@"Track Total\W+3", result.Output);
        Assert.Matches(@"Disc\W+1", result.Output);
        Assert.Matches(@"Disc Total\W+1", result.Output);
    }
}

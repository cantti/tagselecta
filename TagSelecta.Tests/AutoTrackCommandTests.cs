using TagSelecta.Actions;
using TagSelecta.BaseCommands;
using Spectre.Console.Testing;

namespace TagSelecta.Tests;

[Collection("Console")]
public class AutoTrackCommandTests
{
    [Fact]
    public void AutoTrackTest()
    {
        // Given
        var app = new CommandAppTester();
        app.SetDefaultCommand<FileCommand<AutoTrackSettings, AutoTrackAction>>();

        // When
        var result = app.Run("./TestData/AutoTrackTest");

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches(@"(1/3)[\s\S]+Track\W+1", result.Output);
        Assert.Matches(@"(2/3)[\s\S]+Track\W+2", result.Output);
        Assert.Matches(@"(3/3)[\s\S]+Track\W+3", result.Output);
    }
}

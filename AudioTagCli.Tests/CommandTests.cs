using AudioTagCli.Commands;
using Spectre.Console.Testing;

namespace AudioTagCli.Tests;

public class CommandTests
{
    [Fact]
    public void ReadTest()
    {
        // Given
        var app = new CommandAppTester();
        app.SetDefaultCommand<ReadCommand>();

        // When
        var result = app.Run("./TestData/Clint Eastwood - School Mate.mp3");

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches(@"Artist\W+Clint Eastwood", result.Output);
    }

    [Fact]
    public void WriteTest()
    {
        // Given
        var app = new CommandAppTester();
        app.SetDefaultCommand<WriteCommand>();

        // When
        var result = app.Run(
            "./TestData/Clint Eastwood - School Mate.mp3",
            "-a",
            "Test Artist",
            "-A",
            "Test Album Artist",
            "-y",
            "2000"
        );

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches(@"Artist\W+Test Artist", result.Output);
        Assert.Matches(@"Album Artist\W+Test Album Artist", result.Output);
        Assert.Matches(@"Year\W+2000", result.Output);
    }

    [Fact]
    public void AutoTrackTest()
    {
        // Given
        var app = new CommandAppTester();

        // set track number to 0 first
        app.SetDefaultCommand<WriteCommand>();
        app.Run("./TestData/Clint Eastwood - School Mate.mp3", "-n", "0");

        app.SetDefaultCommand<AutoTrackCommand>();
        var result = app.Run("./TestData/Clint Eastwood - School Mate.mp3");

        // Then
        Assert.Equal(0, result.ExitCode);
        Assert.Matches(@"Track\W+1", result.Output);
    }
}

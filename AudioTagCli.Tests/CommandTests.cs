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
        Assert.Contains("Artist         │ Clint Eastwood", result.Output);
    }
}

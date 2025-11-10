using TagSelecta.Cli.Commands.FileCommands;
using TagSelecta.Cli.Tests.Utils;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class ReadTests
{
    [Fact]
    public Task ReadTest()
    {
        var app = CommandAppFactory.CreateTestApp<FileCommand<ReadSettings>>();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run("./TestData/ReadTest/01 Song 1.mp3");

        return Verify(result.Output);
    }
}

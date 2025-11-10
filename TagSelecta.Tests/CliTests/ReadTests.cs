using TagSelecta.Actions.FileActions;
using TagSelecta.Commands;

namespace TagSelecta.Tests.CliTests;

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

using TagSelecta.Cli.Commands.TagDataCommands;
using TagSelecta.Cli.Tests.Utils;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class TitleCaseTests
{
    [Fact]
    public Task TitleCaseTest()
    {
        var app = CommandAppFactory.CreateTestApp<TagDataCommand<TitleCaseSettings>>();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run("./TestData/TitleCaseTest/01 Song 1.mp3");

        return Verify(result.Output);
    }
}

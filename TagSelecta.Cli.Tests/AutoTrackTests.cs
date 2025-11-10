using TagSelecta.Cli.Tests.Utils;
using TagSelecta.Commands.TagDataCommands;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class AutoTrackTests
{
    [Fact]
    public Task AutoTrackTest()
    {
        var app = CommandAppFactory.CreateTestApp<TagDataCommand<AutoTrackSettings>>();
        app.Console.Input.PushTextWithEnter("y");
        app.Console.Input.PushTextWithEnter("y");
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run("./TestData/AutoTrackTest");

        return Verify(result.Output);
    }
}

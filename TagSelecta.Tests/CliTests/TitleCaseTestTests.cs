using TagSelecta.Commands;
using TagSelecta.TagDataActions;

namespace TagSelecta.Tests.CliTests;

[Collection("Console")]
public class TitleCaseTests
{
    [Fact]
    public Task TitleCaseTest()
    {
        var app = CommandAppFactory.CreateTestApp<
            TagDataCommand<TitleCaseAction, TitleCaseSettings>
        >();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run("./TestData/TitleCaseTest/01 Song 1.mp3");

        return Verify(result.Output);
    }
}

using TagSelecta.Commands;
using TagSelecta.TagDataActions;

namespace TagSelecta.Tests.CliTests;

[Collection("Console")]
public class SplitTests
{
    [Fact]
    public Task SplitTest()
    {
        var app = CommandAppFactory.CreateTestApp<TagDataCommand<SplitSettings>>();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run("./TestData/SplitTest/01 Song 1.mp3");

        Console.WriteLine(result.Output);
        return Verify(result.Output);
    }
}

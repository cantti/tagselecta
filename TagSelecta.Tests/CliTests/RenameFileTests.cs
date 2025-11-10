using TagSelecta.Actions.FileActions;
using TagSelecta.Commands;

namespace TagSelecta.Tests.CliTests;

[Collection("Console")]
public class RenameFileTests
{
    [Fact]
    public Task RenameFileTest()
    {
        var app = CommandAppFactory.CreateTestApp<FileCommand<RenameFileSettings>>();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run(
            "./TestData/RenameFileTest/01 Song 1.mp3",
            "-t",
            "{{year}} - {{title}} - {{album}}"
        );

        return Verify(result.Output);
    }
}

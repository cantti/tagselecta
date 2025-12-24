using TagSelecta.Cli.Commands.FileCommands;
using TagSelecta.Cli.Tests.Utils;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class RenameFileTests
{
    [Fact]
    public void RenameFileTest()
    {
        var app = CommandAppFactory.CreateTestApp<FileCommand<RenameFileSettings>>();

        var result = app.Run(
            "./TestData/Album/01 Song 1.mp3",
            "-t",
            "{{date}} - {{title}} - {{album}}"
        );

        Assert.Contains("Status: success", result.Output);
    }
}

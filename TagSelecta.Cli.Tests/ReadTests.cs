using TagSelecta.Cli.Commands.FileCommands;
using TagSelecta.Cli.Tests.Utils;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class ReadTests
{
    [Fact]
    public void ReadTest()
    {
        var app = CommandAppFactory.CreateTestApp<FileCommand<ReadSettings>>();

        var result = app.Run("./TestData/Album/01 Song 1.mp3");

        Assert.Contains("Finished!", result.Output);
    }
}

using TagSelecta.Commands;

namespace TagSelecta.Tests.CliTests;

[Collection("Console")]
public class WriteTests
{
    [Fact]
    public Task WriteTest()
    {
        var app = CommandAppFactory.CreateTestApp<WriteCommand>();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run(
            "./TestData/WriteTest/01 Song 1.mp3",
            "-a",
            "New Test Artist",
            "-A",
            "New Test Album Artist",
            "-t",
            "New Song 1",
            "-l",
            "New Test Album",
            "-y",
            "2000",
            "-g",
            "Reggae",
            "-g",
            "Dub",
            "-n",
            "10",
            "-N",
            "20",
            "-d",
            "30",
            "-D",
            "40"
        );

        return Verify(result.Output);
    }
}

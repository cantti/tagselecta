using NSubstitute;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.RenameFile;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class RenameFileTests
{
    [Fact]
    public void RenameFileTest()
    {
        // Arrange
        var console = Substitute.For<IAnsiConsole>();
        console
            .Input.ReadKey(false)
            .ReturnsForAnyArgs(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        var fs = Substitute.For<IFileSystem>();
        var scanner = Substitute.For<IAudioFileScanner>();
        scanner.Scan(new List<string>()).ReturnsForAnyArgs(["/file1.mp3"]);
        var tagger = Substitute.For<ITagger>();
        tagger
            .ReadTags(Arg.Any<string>())
            .Returns(
                new TagData
                {
                    Date = "1990",
                    Album = "Test Album",
                    Artist = ["Test Artist"],
                }
            );
        var command = new RenameFileCommand(console, tagger, fs, scanner);
        var settings = new RenameFileSettings
        {
            Template = "{{ date }} - {{ artist }} - {{ album }}",
        };

        // Act
        command.Execute(
            new CommandContext(new List<string>(), Substitute.For<IRemainingArguments>(), "", null),
            settings,
            CancellationToken.None
        );

        // Assert
        fs.Received().Move("/file1.mp3", "/1990 - Test Artist - Test Album.mp3");
    }

    [Fact]
    public void GetNewPath_ReturnsPathWithFormattedAndCleanedFileName()
    {
        var settings = new RenameFileSettings { Template = "{{artist}} - {{title}}" };

        var file = new FileWithTagData
        {
            Path = Path.Combine("/Music", "oldname.mp3"),
            TagData = new TagData { Artist = ["Artist"], Title = "Title" },
        };

        var result = FileRenamer.GetNewPath(settings, file);

        var expectedFileName = "Artist - Title.mp3";
        var expectedPath = Path.Combine("/Music", expectedFileName);

        Assert.Equal(expectedPath, result);
    }
}

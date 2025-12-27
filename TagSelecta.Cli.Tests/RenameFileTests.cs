using NSubstitute;
using Spectre.Console;
using TagSelecta.Cli.Commands.FileCommands;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class RenameFileTests
{
    [Fact]
    public async Task RenameFileTest()
    {
        // Arrange
        var console = Substitute.For<IAnsiConsole>();
        var fs = Substitute.For<IFileSystem>();
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
        var action = new RenameFileAction(console, fs, tagger);
        var context = Substitute.For<IFileActionContext<RenameFileSettings>>();
        context.Settings.Returns(
            new RenameFileSettings { Template = "{{ date }} - {{ artist }} - {{ album }}" }
        );
        context.ConfirmPrompt().Returns(true);
        context.CurrentFile.Returns("/file1.mp3");
        context.CurrentFileIndex.Returns(0);

        // Act
        await action.ProcessFileAsync(context);

        // Assert
        fs.Received().Move("/file1.mp3", "/1990 - Test Artist - Test Album.mp3");
    }
}

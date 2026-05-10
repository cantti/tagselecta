using TagSelecta.Commands.Tui;

namespace TagSelecta.Commands.Tests;

public class CommandParserTests
{
    [Fact]
    public void TryParse_ValidSingleCommandWithOptions_ReturnsParsedCommand()
    {
        var success = CommandParser.TryParse(
            "write key=title value=\"Hello World\" force",
            out var parsedCommands
        );

        Assert.True(success);
        Assert.Single(parsedCommands);

        var command = parsedCommands[0];
        Assert.Equal("write", command.Name);
        Assert.Equal(3, command.Options.Count);

        var options = command.Options;
        Assert.Equal(3, options.Count);
        Assert.Equal("title", options.Single(x => x.Key == "key").Value);
        Assert.Equal("Hello World", options.Single(x => x.Key == "value").Value);
        Assert.Equal(string.Empty, options.Single(x => x.Key == "force").Value);
    }

    [Fact]
    public void TryParse_ValidChainedCommands_ReturnsAllCommands()
    {
        var success = CommandParser.TryParse(
            "selectall && write key=genre value=Rock && quit",
            out var parsedCommands
        );

        Assert.True(success);
        Assert.Equal(3, parsedCommands.Count);
        Assert.Equal("selectall", parsedCommands[0].Name);
        Assert.Equal("write", parsedCommands[1].Name);
        Assert.Equal("quit", parsedCommands[2].Name);
    }

    [Fact]
    public void TryParse_InvalidCommand_ReturnsFalseAndEmptyCommands()
    {
        var success = CommandParser.TryParse("123invalid", out var parsedCommands);

        Assert.False(success);
        Assert.Empty(parsedCommands);
    }
}

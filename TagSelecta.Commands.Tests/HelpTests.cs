using System.Reflection;
using Spectre.Console;
using Spectre.Console.Rendering;
using TagSelecta.Commands.Tui;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.Commands.Tui.Widgets;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tests;

public class HelpTests
{
    [Theory]
    [InlineData("help", "")]
    [InlineData("h", "")]
    [InlineData("help edit", "edit")]
    [InlineData("h edit", "edit")]
    public async Task HelpCommand_OpensRequestedTopic(string command, string topic)
    {
        var dispatcher = new TuiCommandDispatcher(new TuiCommandFactory(new TestServices()));
        var context = new TestContext { CommandHelpEnabled = true, HelpTopic = "keymap" };

        Assert.True(await dispatcher.Execute(command, context, CancellationToken.None));
        Assert.True(context.CommandHelpEnabled);
        Assert.Equal(topic, context.HelpTopic);
    }

    [Fact]
    public async Task HelpViews_CanBeOpenedAndClosed()
    {
        var context = new TestContext();
        var help = new HelpCommand();
        var open = new ToggleCommandHelpCommand();
        var escape = new EscapeCommand();

        await help.ExecuteAsync(
            context,
            new ParsedCommand("help", [new("edit", "")]),
            CancellationToken.None
        );
        await open.ExecuteAsync(
            context,
            new ParsedCommand("togglecommandhelp", []),
            CancellationToken.None
        );
        Assert.True(context.CommandHelpEnabled);
        Assert.Equal("", context.HelpTopic);
        await open.ExecuteAsync(
            context,
            new ParsedCommand("togglecommandhelp", []),
            CancellationToken.None
        );
        Assert.True(context.CommandHelpEnabled);
        Assert.Equal("", context.HelpTopic);
        await escape.ExecuteAsync(context, new ParsedCommand("escape", []), CancellationToken.None);
        Assert.False(context.CommandHelpEnabled);
    }

    [Theory]
    [InlineData("keymap", true)]
    [InlineData("KEYMAP", true)]
    [InlineData("edit", true)]
    [InlineData("", false)]
    [InlineData("keymap", false)]
    public async Task KeymapHelp_OpensKeymap(string topic, bool helpEnabled)
    {
        var context = new TestContext { CommandHelpEnabled = helpEnabled, HelpTopic = topic };
        var open = new ToggleKeymapHelpCommand();
        await open.ExecuteAsync(
            context,
            new ParsedCommand("togglekeymaphelp", []),
            CancellationToken.None
        );
        Assert.True(context.CommandHelpEnabled);
        Assert.Equal("keymap", context.HelpTopic);
        await open.ExecuteAsync(
            context,
            new ParsedCommand("togglekeymaphelp", []),
            CancellationToken.None
        );
        Assert.True(context.CommandHelpEnabled);
        Assert.Equal("keymap", context.HelpTopic);
    }

    [Theory]
    [InlineData("keymap", true)]
    [InlineData("KEYMAP", true)]
    [InlineData("edit", true)]
    [InlineData("", true)]
    [InlineData("keymap", false)]
    public async Task CommandHelp_OpensGeneralHelp(string topic, bool helpEnabled)
    {
        var context = new TestContext { CommandHelpEnabled = helpEnabled, HelpTopic = topic };
        await new ToggleCommandHelpCommand().ExecuteAsync(
            context,
            new ParsedCommand("togglecommandhelp", []),
            CancellationToken.None
        );
        Assert.True(context.CommandHelpEnabled);
        Assert.Equal("", context.HelpTopic);
    }

    [Fact]
    public async Task Escape_ClosesHelpAndClearsSelection()
    {
        var file = new TagDataActionTarget("file.mp3", new TagData()) { IsSelected = true };
        var context = new TestContext
        {
            CommandHelpEnabled = true,
            HelpTopic = "edit",
            SelectedFiles = [file],
        };
        await new EscapeCommand().ExecuteAsync(
            context,
            new ParsedCommand("escape", []),
            CancellationToken.None
        );
        Assert.False(context.CommandHelpEnabled);
        Assert.Equal("", context.HelpTopic);
        Assert.False(file.IsSelected);
    }

    [Theory]
    [InlineData("", ":help <command>")]
    [InlineData("edit", "Updates fields on selected files")]
    [InlineData("e", "Updates fields on selected files")]
    [InlineData("edit", "Standard fields:")]
    [InlineData("edit", "albumartist")]
    [InlineData("e", "albumartist")]
    [InlineData("templates", "Scriban templates")]
    [InlineData("template", ":edit title=\"{{ filename }}\"")]
    [InlineData("TEMPLATES", "pad(tracknumber)")]
    [InlineData("edit", ":help templates")]
    [InlineData("move", ":help templates")]
    [InlineData("fields", "Unknown help topic: fields")]
    [InlineData("keymap", "Keymap help")]
    [InlineData("KEYMAP", "Show keymap help")]
    [InlineData("keymap", "Clear selection and close help or picture")]
    [InlineData("selectall", "Select all files")]
    [InlineData("undo", "Undo changes")]
    [InlineData("q", "Exit the TUI")]
    [InlineData("version", "current and latest")]
    [InlineData("togglefilter", "filter for changed files")]
    [InlineData("autotrack", "keepdisk")]
    [InlineData("extractpicture", "written immediately")]
    [InlineData("notfound", "Unknown help topic: notfound")]
    [InlineData("[unknown]", "Unknown help topic: [unknown]")]
    public void HelpWidget_RendersTopic(string topic, string expected)
    {
        using var writer = new StringWriter();
        var console = AnsiConsole.Create(
            new AnsiConsoleSettings { Ansi = AnsiSupport.No, Out = new AnsiConsoleOutput(writer) }
        );
        console.Profile.Width = 120;
        console.Write(new HelpWidget(new TuiAppTheme(), topic));

        Assert.Contains(expected, writer.ToString());
    }

    [Theory]
    [InlineData("", "Show help", false)]
    [InlineData("", ":help", true)]
    [InlineData("edit", "Updates fields", false)]
    [InlineData("edit", "Help: edit", false)]
    [InlineData("edit", "Standard fields:", false)]
    [InlineData("edit", ":edit", true)]
    [InlineData("keymap", "Command mode", false)]
    [InlineData("keymap", ":", true)]
    [InlineData("templates", "Scriban templates", false)]
    [InlineData("templates", ":edit title", false)]
    [InlineData("notfound", "Unknown help topic", false)]
    public void HelpWidget_UsesThemeColors(string topic, string text, bool accent)
    {
        var theme = new TuiAppTheme { TextFg = "#123456", AccentFg = "#234567" };
        var console = AnsiConsole.Create(new AnsiConsoleSettings { Ansi = AnsiSupport.No });
        var options = new RenderOptions(console.Profile.Capabilities, new Size(120, 50));
        var segments = new HelpWidget(theme, topic).ToSegments(options, 120);
        var color = Color.FromHex(accent ? theme.AccentFg : theme.TextFg);

        var coloredText = string.Concat(
            segments.Where(x => x.Style.Foreground == color).Select(x => x.Text)
        );
        Assert.Contains(text, coloredText);
    }

    [Fact]
    public void HelpTopics_CoverTuiCommands()
    {
        var commandNames = typeof(ITuiCommand)
            .Assembly.GetTypes()
            .Select(type => type.GetCustomAttribute<TuiCommandAttribute>())
            .Where(attribute => attribute is not null && attribute.Names[0] != "execute")
            .Select(attribute => attribute!.Names[0]);
        Assert.Empty(commandNames.Except(HelpWidget.Topics));
    }

    [Theory]
    [InlineData("edit title=\"{{ filename }}\"", "song")]
    [InlineData("edit genre=\"{{ genre | string.downcase }}\"", "reggae")]
    [InlineData(
        "move template=\"../{{ year }} - {{ album }}/{{ pad(tracknumber) }}. {{ title }}.{{ ext }}\"",
        "../2020 - Album/01. Track.mp3"
    )]
    public void TemplateHelp_ExamplesCanBeParsedAndRendered(string command, string expected)
    {
        var tagData = new TagData();
        tagData.SetValue("genre", "Reggae");
        tagData.SetValue("date", "2020");
        tagData.SetValue("album", "Album");
        tagData.SetValue("tracknumber", "1");
        tagData.SetValue("title", "Track");

        Assert.True(CommandParser.TryParse(command, out var commands));
        var template = Assert.Single(Assert.Single(commands).Options).Value;
        var formatter = new TagDataFormatter(tagData, "/music/song.mp3");
        Assert.Equal(expected, formatter.Format(template));
    }

    private class TestServices : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IEnumerable<ITuiCommand>)
                ? new ITuiCommand[] { new HelpCommand() }
                : null;
        }
    }

    private class TestContext : ITuiCommandContext
    {
        public int FocusedFileIndex { get; set; }
        public int MetadataOffset { get; set; }
        public IEnumerable<TagDataActionTarget> Files { get; private set; } = [];
        public IEnumerable<TagDataActionTarget> SelectedFiles { get; set; } = [];
        public TagDataActionTarget? FocusedFile { get; }
        public bool TreeEnabled { get; set; }
        public bool FilterEnabled { get; set; }
        public bool CommandHelpEnabled { get; set; }
        public string HelpTopic { get; set; } = "";
        public bool FileListEnabled { get; set; }
        public IEnumerable<TagDataActionTarget> VisibleFiles { get; } = [];
        public bool PictureEnabled { get; set; }

        public void Quit() { }

        public void Print(string markupMessage) { }

        public void SetCommandPromptText(string text) { }

        public void SetFiles(IEnumerable<TagDataActionTarget> files)
        {
            Files = files;
        }
    }
}

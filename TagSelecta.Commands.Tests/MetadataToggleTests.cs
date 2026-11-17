using System.Reflection;
using Spectre.Console;
using TagSelecta.Commands.Tui;
using TagSelecta.Commands.Tui.Completion;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.Commands.Tui.Widgets;

namespace TagSelecta.Commands.Tests;

public class MetadataToggleTests
{
    [Fact]
    public async Task ToggleMetadata_HidesAndRestoresMetadataWithoutChangingOtherState()
    {
        var app = CreateApp();
        app.MetadataOffset = 3;
        var dispatcher = new TuiCommandDispatcher(new TuiCommandFactory(new TestServices()));

        Assert.True(app.MetadataEnabled);
        Assert.True(await dispatcher.Execute("togglemetadata", app, CancellationToken.None));
        Assert.False(app.MetadataEnabled);
        Assert.True(app.FileListEnabled);
        Assert.Equal(3, app.MetadataOffset);

        Assert.True(await dispatcher.Execute("togglemetadata", app, CancellationToken.None));
        Assert.True(app.MetadataEnabled);
        Assert.True(app.FileListEnabled);
        Assert.Equal(3, app.MetadataOffset);
    }

    [Fact]
    public void MetadataHotkey_DispatchesToggleMetadata()
    {
        var hotkeys = new HotkeyMap();
        var completionProvider = new CompletionProvider([], new MacroConfig());
        var inputHandler = new InputHandler(hotkeys, completionProvider);
        var app = new TuiApp(
            AnsiConsole.Create(new AnsiConsoleSettings()),
            null!,
            null!,
            new TuiAppConfig(),
            inputHandler,
            hotkeys,
            completionProvider
        );
        typeof(TuiApp)
            .GetMethod("BindHotkeys", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(app, null);

        Assert.True(
            inputHandler.ProcessKey(
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                out var command
            )
        );
        Assert.Equal("togglemetadata", command);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Layout_RespectsIndependentPanelVisibility(
        bool metadataEnabled,
        bool fileListEnabled
    )
    {
        var app = CreateApp();
        app.MetadataEnabled = metadataEnabled;
        app.FileListEnabled = fileListEnabled;

        var layout = DrawLayout(app);

        Assert.Equal(metadataEnabled, Render(layout).Contains("Metadata (0)"));
        if (fileListEnabled)
        {
            Assert.NotNull(layout.GetLayout("files"));
        }
        else
        {
            Assert.Throws<InvalidOperationException>(() => layout.GetLayout("files"));
        }

        Assert.NotNull(layout.GetLayout("status"));
        if (!metadataEnabled && fileListEnabled)
        {
            Assert.Equal(
                Math.Max(1, Console.WindowHeight - SectionPanel.VerticalPadding - 1),
                layout["files"].Size
            );
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Layout_HelpRemainsAvailableWithMetadataHidden(bool fileListEnabled)
    {
        var app = CreateApp();
        app.MetadataEnabled = false;
        app.FileListEnabled = fileListEnabled;
        app.CommandHelpEnabled = true;

        Assert.Contains("Help", Render(DrawLayout(app)));

        app.CommandHelpEnabled = false;
        Assert.DoesNotContain("Metadata (0)", Render(DrawLayout(app)));
    }

    private static TuiApp CreateApp()
    {
        var hotkeys = new HotkeyMap();
        var completionProvider = new CompletionProvider([], new MacroConfig());
        return new TuiApp(
            AnsiConsole.Create(new AnsiConsoleSettings()),
            null!,
            null!,
            new TuiAppConfig { FileListRatio = 0.5 },
            new InputHandler(hotkeys, completionProvider),
            hotkeys,
            completionProvider
        );
    }

    private static Layout DrawLayout(TuiApp app)
    {
        return (Layout)
            typeof(TuiApp)
                .GetMethod("DrawLayout", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(app, null)!;
    }

    private static string Render(Layout layout)
    {
        using var writer = new StringWriter();
        var console = AnsiConsole.Create(
            new AnsiConsoleSettings { Ansi = AnsiSupport.No, Out = new AnsiConsoleOutput(writer) }
        );
        console.Profile.Width = 120;
        console.Profile.Height = 40;
        console.Write(layout);
        return writer.ToString();
    }

    private class TestServices : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IEnumerable<ITuiCommand>)
                ? new ITuiCommand[] { new ToggleMetadataCommand() }
                : null;
        }
    }
}

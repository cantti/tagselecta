using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using TagSelecta.App.Config;
using TagSelecta.Commands.Tui;
using TagSelecta.Commands.Tui.Widgets;
using TagSelecta.Shared.Tagging;
using Tomlyn;

namespace TagSelecta.Commands.Tests;

public class AppearanceConfigTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FieldIconsEnabled_ControlsMetadataIcons(bool enabled)
    {
        var toml = $"""
            [appearance]
            field_icons_enabled = {enabled.ToString().ToLowerInvariant()}

            [field_icons]
            artist = "ARTICON"
            """;
        var model = Toml.ToModel<ConfigModel>(toml);
        var services = new ServiceCollection();
        services.AddSettings(model);
        using var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<TuiConfig>();
        var tags = new TagData();
        tags.SetValue("artist", "Example Artist");
        var file = new TagDataActionTarget("song.mp3", tags);
        using var writer = new StringWriter();
        var console = AnsiConsole.Create(
            new AnsiConsoleSettings { Ansi = AnsiSupport.No, Out = new AnsiConsoleOutput(writer) }
        );
        console.Profile.Width = 120;

        console.Write(new TagDataWidget(config, file, 0));

        Assert.Equal(enabled, config.Appearance.FieldIconsEnabled);
        Assert.Contains("Example Artist", writer.ToString());
        Assert.Equal(enabled, writer.ToString().Contains("ARTICON"));
        Assert.True(model.FieldIcons.ContainsKey("title"));
    }

    [Fact]
    public void FieldIcons_AreDisabledByDefault()
    {
        var model = Toml.ToModel<ConfigModel>("");
        Assert.False(model.Appearance.FieldIconsEnabled);
    }
}

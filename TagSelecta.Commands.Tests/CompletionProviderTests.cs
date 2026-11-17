using TagSelecta.Commands.Tui.Completion;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.TagDataActions.Clear;
using TagSelecta.TagDataActions.Split;

namespace TagSelecta.Commands.Tests;

public class CompletionProviderTests
{
    [Theory]
    [InlineData("ru", "n")]
    [InlineData("selecta", "ll")]
    [InlineData("togglemeta", "data")]
    [InlineData("help togglemeta", "data")]
    [InlineData("help edi", "t")]
    [InlineData("help templ", "ates")]
    [InlineData("h templ", "ate")]
    [InlineData("h key", "map")]
    [InlineData("help selec", "tall")]
    [InlineData("run reg", "gae")]
    [InlineData("r reg", "gae")]
    [InlineData("macro reg", "gae")]
    [InlineData("m reg", "gae")]
    [InlineData("split sep", "arator=")]
    public void GetCompletions_SuggestsCommandsOptionsAndTopics(string input, string suffix)
    {
        var provider = CreateProvider();
        provider.GenerateCompletions([]);
        Assert.Contains(suffix, provider.GetCompletions(input, input.Length));
    }

    [Fact]
    public void GenerateCompletions_ReplacesOldSuggestions()
    {
        var provider = CreateProvider();
        provider.GenerateCompletions([]);
        provider.GenerateCompletions([]);
        Assert.Single(provider.GetCompletions("run reg", "run reg".Length));
    }

    [Fact]
    public void GenerateCompletions_RefreshesFieldNames()
    {
        var provider = new CompletionProvider([new ClearAction()], new MacroConfig());
        provider.GenerateCompletions(["oldfield"]);
        provider.GenerateCompletions(["newfield"]);
        Assert.Empty(provider.GetCompletions("clear old", "clear old".Length));
        Assert.Contains("field", provider.GetCompletions("clear new", "clear new".Length));
    }

    [Fact]
    public void GetCompletions_DoesNotSuggestInternalDispatcherCommand()
    {
        var provider = CreateProvider();
        provider.GenerateCompletions([]);
        Assert.Empty(provider.GetCompletions("execut", "execut".Length));
    }

    [Fact]
    public void GetCompletions_DoesNotSuggestFieldsTopic()
    {
        var provider = CreateProvider();
        provider.GenerateCompletions([]);
        Assert.Empty(provider.GetCompletions("help fields", "help fields".Length));
    }

    private static CompletionProvider CreateProvider()
    {
        return new CompletionProvider(
            [new SplitAction()],
            new MacroConfig
            {
                Macros = new Dictionary<string, string> { ["reggae"] = "edit genre=Reggae" },
            }
        );
    }
}

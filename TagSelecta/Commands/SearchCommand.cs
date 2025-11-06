using System.ComponentModel;
using System.Linq.Dynamic.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class SearchSettings : FileSettings
{
    [CommandOption("--query|-q")]
    [Description("Search query")]
    public string? Query { get; set; } = "";
}

public class SearchCommand(IAnsiConsole console) : FileCommand<SearchSettings>(console)
{
    protected override Task BeforeExecute()
    {
        var tags = Files.Select(x => new { Path = x, TagData = Tagger.ReadTags(x) }).ToList();
        Parallel.ForEach(
            Files,
            file =>
            {
                var tagData = Tagger.ReadTags(file);
                var lambda = DynamicExpressionParser.ParseLambda<TagData, bool>(
                    new ParsingConfig(),
                    false,
                    Settings.Query ?? ""
                );
                var result = lambda.Compile().Invoke(tagData);
                if (result)
                {
                    Console.WriteLine(file);
                }
            }
        );
        Cancel();
        return Task.CompletedTask;
    }

    protected override Task Execute(string file, int index)
    {
        return Task.CompletedTask;
    }
}

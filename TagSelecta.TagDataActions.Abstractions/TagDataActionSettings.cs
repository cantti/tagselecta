using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.TagDataActions.Abstractions;

public abstract class TagDataActionSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];

    [CommandOption("--yes")]
    [Description("Skip confirmation before writing changes to files.")]
    public bool Yes { get; set; }

    public List<RemainingArgument> Remaining { get; set; } = [];
}

public class RemainingArgument
{
    public RemainingArgument(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string Value { get; }
}

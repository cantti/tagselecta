using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Split;

public class SplitSettings : TagDataActionSettings
{
    [CommandOption("--separator|-s")]
    [Description("Separator. Can be used multiple times.")]
    [DefaultValue("[\",\", \";\", \"feat.\"]")]
    public string[] Separator { get; set; } = [",", ";", "feat."];
}

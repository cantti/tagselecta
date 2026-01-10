using System.ComponentModel;
using Spectre.Console.Cli;

namespace TagSelecta.App.CliCommands.ExtractPicture;

public class ExtractPictureSettings : BaseSettings
{
    [CommandOption("--type|-t")]
    [Description(
        "Types of pictures to extract. Multiple entries can be provided using a ';' separator.\nCommon types: FrontCover, BackCover, Artist, Other"
    )]
    public string? Type { get; set; }

    [CommandOption("--output|-o")]
    [Description("Output file name")]
    public string? Output { get; set; }

    [CommandOption("--override")]
    [Description("Override files")]
    public bool Override { get; set; }

    [CommandOption("--limit|-l")]
    [Description("Limit number of files to be extracted")]
    public int? Limit { get; set; }
}

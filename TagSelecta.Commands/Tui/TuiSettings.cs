using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.IO;

namespace TagSelecta.Commands.Tui;

public class TuiSettings : CommandSettings
{
    [CommandArgument(0, "[path]")]
    public string[] Path { get; set; } = ["."];

    [CommandOption("--no-recursive")]
    [Description("Do not scan subdirectories.")]
    public bool NoRecursive { get; set; }

    public override ValidationResult Validate()
    {
        foreach (var path in Path)
        {
            if (!PathUtils.Exists(PathUtils.Expand(path)))
            {
                return ValidationResult.Error($"Path not found: {path}");
            }
        }

        return ValidationResult.Success();
    }
}

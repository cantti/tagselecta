#!/usr/bin/dotnet run
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        const string mdPath = "./docs/docs/05-cli/03-cli-commands.md";
        string readme = File.ReadAllText(mdPath);
        readme = UpdateHelp(readme);
        File.WriteAllText(mdPath, readme);
    }

    static string UpdateHelp(string mdPath)
    {
        Console.WriteLine("Updating README.md CLI help section...\n");

        const string helpStart = "<!-- start:cli-help -->";
        const string helpEnd = "<!-- end:cli-help -->";

        // Define all CLI commands to document
        var commands = new (string DisplayName, string CommandLine)[]
        {
            ("Help", "run -p:WarningLevel=0 --project ./TagSelecta.App -- --help"),
            ("Edit", "run -p:WarningLevel=0 --project ./TagSelecta.App -- edit --help"),
            ("Move", "run -p:WarningLevel=0 --project ./TagSelecta.App -- move --help"),
            ("Split", "run -p:WarningLevel=0 --project ./TagSelecta.App -- split --help"),
            ("Title case", "run -p:WarningLevel=0 --project ./TagSelecta.App -- titlecase --help"),
            ("Auto Track", "run -p:WarningLevel=0 --project ./TagSelecta.App -- autotrack --help"),
            ("Find", "run -p:WarningLevel=0 --project ./TagSelecta.App -- find --help"),
            ("Discogs", "run -p:WarningLevel=0 --project ./TagSelecta.App -- discogs --help"),
            (
                "MusicBrainz",
                "run -p:WarningLevel=0 --project ./TagSelecta.App -- musicbrainz --help"
            ),
            ("Clear", "run -p:WarningLevel=0 --project ./TagSelecta.App -- clear --help"),
            (
                "ClearExcept",
                "run -p:WarningLevel=0 --project ./TagSelecta.App -- clearexcept --help"
            ),
            (
                "Extract Picture",
                "run -p:WarningLevel=0 --project ./TagSelecta.App -- extractpicture --help"
            ),
        };

        // Generate new content for md
        var sb = new StringBuilder();
        foreach (var (name, commandLine) in commands)
        {
            Console.WriteLine($"Capturing help for {name}...");
            string output = Run("dotnet", commandLine);
            output = output.Replace("tagselecta.dll", "tagselecta");
            sb.AppendLine($"### {name} command\n");
            sb.AppendLine("```");
            sb.AppendLine(output);
            sb.AppendLine("```");
            sb.AppendLine();
        }

        Console.WriteLine("\nInserting all help outputs into md...");

        if (!mdPath.Contains(helpStart) || !mdPath.Contains(helpEnd))
        {
            throw new Exception(
                "Markers <!-- start:cli-help --> and <!-- end:cli-help --> not found in md"
            );
        }

        // Replace everything between markers
        string updated = Regex.Replace(
            mdPath,
            $"{Regex.Escape(helpStart)}.*?{Regex.Escape(helpEnd)}",
            $"{helpStart}\n{sb}\n{helpEnd}",
            RegexOptions.Singleline
        );

        Console.WriteLine("Markdown file updated successfully!");

        return updated;
    }

    static string Run(string cmd, string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = cmd,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        psi.Environment["TAGSELECTA_NOANSI"] = "1";

        using var p = Process.Start(psi)!;
        string output = p.StandardOutput.ReadToEnd();
        string err = p.StandardError.ReadToEnd();
        p.WaitForExit();

        if (p.ExitCode != 0)
            throw new Exception($"Command failed: {cmd} {args}\n{err}");

        return output.Trim();
    }
}

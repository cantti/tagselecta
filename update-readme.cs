#!/usr/bin/dotnet run
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        const string readmePath = "README.md";
        string readme = File.ReadAllText(readmePath);
        readme = UpdateHelp(readme);
        readme = UpdateFormatting(readme);
        File.WriteAllText(readmePath, readme);
    }

    static string UpdateHelp(string readme)
    {
        Console.WriteLine("Updating README.md CLI help section...\n");

        const string helpStart = "<!-- start:cli-help -->";
        const string helpEnd = "<!-- end:cli-help -->";

        // Define all CLI commands to document
        var commands = new (string DisplayName, string CommandLine)[]
        {
            ("Help", "run --project ./TagSelecta.Cli -- --help"),
            ("Read", "run --project ./TagSelecta.Cli -- read --help"),
            ("Write", "run --project ./TagSelecta.Cli -- write --help"),
            ("Split", "run --project ./TagSelecta.Cli -- split --help"),
            ("Auto Track", "run --project ./TagSelecta.Cli -- autotrack --help"),
            ("Rename Directory", "run --project ./TagSelecta.Cli -- renamedir --help"),
            ("Rename File", "run --project ./TagSelecta.Cli -- renamefile --help"),
            ("Fix Album", "run --project ./TagSelecta.Cli -- fixalbum --help"),
            ("Discogs", "run --project ./TagSelecta.Cli -- discogs --help"),
            ("Discogs", "run --project ./TagSelecta.Cli -- discogs --help"),
            ("Extract Picture", "run --project ./TagSelecta.Cli -- extractpicture --help"),
            ("Find", "run --project ./TagSelecta.Cli -- find --help"),
        };

        // Generate new content for README
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

        Console.WriteLine("\nInserting all help outputs into README...");

        if (!readme.Contains(helpStart) || !readme.Contains(helpEnd))
        {
            throw new Exception(
                "Markers <!-- start:cli-help --> and <!-- end:cli-help --> not found in README.md"
            );
        }

        // Replace everything between markers
        string updated = Regex.Replace(
            readme,
            $"{Regex.Escape(helpStart)}.*?{Regex.Escape(helpEnd)}",
            $"{helpStart}\n{sb}\n{helpEnd}",
            RegexOptions.Singleline
        );

        Console.WriteLine("README.md updated successfully!");

        return updated;
    }

    static string UpdateFormatting(string readme)
    {
        Console.WriteLine("Updating README.md formatting section...\n");

        const string start = "<!-- start:formatting -->";
        const string end = "<!-- end:formatting -->";
        string output = Run("dotnet", "run --project ./TagSelecta.Cli -- helpformatting");
        string updated = Regex.Replace(
            readme,
            $"{Regex.Escape(start)}.*?{Regex.Escape(end)}",
            $"{start}\n```\n{output}\n```\n{end}",
            RegexOptions.Singleline
        );
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

#!/usr/bin/dotnet run
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

const string readmePath = "README.md";
const string START = "<!-- start:cli-help -->";
const string END = "<!-- end:cli-help -->";

Console.WriteLine("Updating README.md CLI help section...\n");

// Define all CLI commands to document
var commands = new (string DisplayName, string CommandLine)[]
{
    ("Help", "run --project ./TagSelecta -- --help"),
    ("Read", "run --project ./TagSelecta -- read --help"),
    ("Write", "run --project ./TagSelecta -- write --help"),
    ("Split", "run --project ./TagSelecta -- split --help"),
    ("Clean", "run --project ./TagSelecta -- clean --help"),
    ("Auto Track", "run --project ./TagSelecta -- autotrack --help"),
    ("Rename Directory", "run --project ./TagSelecta -- renamedir --help"),
    ("Rename File", "run --project ./TagSelecta -- renamefile --help"),
    ("Fix Album", "run --project ./TagSelecta -- fixalbum --help"),
    ("Discogs", "run --project ./TagSelecta -- discogs --help"),
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

string readme = File.ReadAllText(readmePath);

if (!readme.Contains(START) || !readme.Contains(END))
{
    throw new Exception(
        "Markers <!-- start:cli-help --> and <!-- end:cli-help --> not found in README.md"
    );
}

// Replace everything between markers
string updated = Regex.Replace(
    readme,
    $"{Regex.Escape(START)}.*?{Regex.Escape(END)}",
    $"{START}\n\n{sb}\n{END}",
    RegexOptions.Singleline
);

File.WriteAllText(readmePath, updated);

Console.WriteLine("README.md updated successfully!");

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
    // psi.Environment["NO_COLOR"] = "1";

    using var p = Process.Start(psi)!;
    string output = p.StandardOutput.ReadToEnd();
    string err = p.StandardError.ReadToEnd();
    p.WaitForExit();

    if (p.ExitCode != 0)
        throw new Exception($"Command failed: {cmd} {args}\n{err}");

    return output.Trim();
}

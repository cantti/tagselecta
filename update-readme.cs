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
    ("Help", "dotnet run --project ./TagSelecta -- --help"),
    ("Read", "dotnet run --project ./TagSelecta -- read --help"),
    ("Write", "dotnet run --project ./TagSelecta -- write --help"),
    ("RenameDir", "dotnet run --project ./TagSelecta -- renamedir --help"),
    ("RenameFile", "dotnet run --project ./TagSelecta -- renamefile --help"),
    ("Clean", "dotnet run --project ./TagSelecta -- clean --help"),
    ("FixAlbum", "dotnet run --project ./TagSelecta -- fixalbum --help"),
    ("AutoTrack", "dotnet run --project ./TagSelecta -- autotrack --help"),
};

// Generate new content for README
var sb = new StringBuilder();
foreach (var (name, commandLine) in commands)
{
    Console.WriteLine($"Capturing help for {name}...");
    string output = Run("dotnet", commandLine.Replace("dotnet ", "")); // split below
    sb.AppendLine($"**{name} command**\n");
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

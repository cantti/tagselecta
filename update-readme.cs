#!/usr/bin/dotnet run
using System.Diagnostics;

string readmePath = "README.md";
string cliCommand = "dotnet run --project ./TagSelecta -- --help"; // Change to your CLI command

Console.WriteLine("Capturing CLI help output...");
string helpOutput = Run(cliCommand);

Console.WriteLine("Reading README.md...");
string readme = File.ReadAllText(readmePath);

const string START = "<!-- CLI_HELP_START -->";
const string END = "<!-- CLI_HELP_END -->";

if (!readme.Contains(START) || !readme.Contains(END))
{
    Console.WriteLine("⚠️  Markers not found, adding them.");
    readme += $"\n\n## Usage\n{START}\n{END}\n";
}

string before = readme.Split(START)[0];
string after = readme.Split(END)[1];

string updated = $"{before}{START}\n```\n{helpOutput}\n```\n{END}{after}";

File.WriteAllText(readmePath, updated);

Console.WriteLine("✅ README.md updated successfully!");

static string Run(string cmd)
{
    var psi = new ProcessStartInfo
    {
        FileName = "bash",
        Arguments = $"-c \"{cmd}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };
    using var p = Process.Start(psi);
    string output = p!.StandardOutput.ReadToEnd();
    string err = p.StandardError.ReadToEnd();
    p.WaitForExit();

    if (p.ExitCode != 0)
        throw new Exception($"Command failed: {cmd}\n{err}");

    return output.Trim();
}

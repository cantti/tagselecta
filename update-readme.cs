#!/usr/bin/dotnet run
using System.Diagnostics;

const string readmePath = "README.md";

Console.WriteLine("📘 Updating README.md CLI sections...\n");

UpdateReadmeSection("help", "dotnet", "run --project ./TagSelecta -- --help");
UpdateReadmeSection("read", "dotnet", "run --project ./TagSelecta -- read --help");

Console.WriteLine("\n✅ All README sections updated successfully!");

static void UpdateReadmeSection(string sectionName, string cmd, string args)
{
    string start = $"<!-- start:{sectionName} -->";
    string end = $"<!-- end:{sectionName} -->";

    Console.WriteLine($"➡️  Updating section: {sectionName}");

    string helpOutput = Run(cmd, args);

    string readme = File.ReadAllText(readmePath);

    // Add section if not present
    if (!readme.Contains(start) || !readme.Contains(end))
    {
        Console.WriteLine($"   ⚠️  Markers for '{sectionName}' not found. Adding them.");
        readme += $"\n\n## {sectionName}\n{start}\n{end}\n";
    }

    // Split file into before/after the markers
    string before = readme.Split(start)[0];
    string after = readme.Split(end)[1];

    // Build new README content
    string updated = $"{before}{start}\n```\n{helpOutput}\n```\n{end}{after}";

    File.WriteAllText(readmePath, updated);
    Console.WriteLine($"   ✅ Section '{sectionName}' updated.\n");
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

    psi.Environment["NO_COLOR"] = "1";
    psi.Environment["DOTNET_ENVIRONMENT"] = "Production";

    using var p = Process.Start(psi)!;
    string output = p.StandardOutput.ReadToEnd();
    string err = p.StandardError.ReadToEnd();
    p.WaitForExit();

    if (p.ExitCode != 0)
        throw new Exception($"Command failed: {cmd} {args}\n{err}");

    return output.Trim();
}

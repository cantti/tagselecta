using System.Diagnostics;
using System.Text.Json.Nodes;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.TagDataActions.Json;

public static class JqWrapper
{
    public static List<JsonNode?> Evaluate(string json, string filter)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "jq",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        process.StartInfo.ArgumentList.Add("-c");
        process.StartInfo.ArgumentList.Add(filter);

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            throw new TagSelectaException(
                $"Failed to start jq. Make sure jq is installed. {ex.Message}"
            );
        }

        process.StandardInput.Write(json);
        process.StandardInput.Close();

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new TagSelectaException($"jq evaluation failed: {stderr}".Trim());
        }

        var result = new List<JsonNode?>();
        var lines = stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            result.Add(JsonNode.Parse(trimmed));
        }

        return result;
    }
}

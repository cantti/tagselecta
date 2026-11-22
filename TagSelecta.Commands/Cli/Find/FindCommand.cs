using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Cli.Find;

public class FindCommand(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    IFindProcessRunner processRunner
) : AsyncCommand<FindSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        FindSettings settings,
        CancellationToken ct
    )
    {
        var input = context.Arguments.ToArray();
        var separator = Array.IndexOf(input, "--");
        var arguments = separator < 0 ? [] : input[(separator + 1)..];
        if (settings.Exec is null && arguments.Length > 0)
        {
            console.WriteLine("Arguments after -- require --exec.");
            return 1;
        }

        if (settings.Exec is not null && string.IsNullOrWhiteSpace(settings.Exec))
        {
            console.WriteLine("A program is required for --exec.");
            return 1;
        }

        if (settings.Exec is not null)
        {
            var unknownOption = FindUnknownOption(context, input, separator);
            if (unknownOption is not null)
            {
                console.WriteLine($"Unknown find option: {unknownOption}");
                return 1;
            }
        }

        var hasPlaceholder = arguments.Any(x => x.Contains("{}", StringComparison.Ordinal));
        try
        {
            ct.ThrowIfCancellationRequested();
            var files = audioFileScanner.SearchAndRead(settings.Path, !settings.NoRecursive, ct);
            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();
                var formatter = new TagDataFormatter(file.TagData, file.Path);
                var shouldPrint =
                    string.IsNullOrWhiteSpace(settings.Query)
                    || formatter.Format(settings.Query) == "true";
                if (!shouldPrint)
                {
                    continue;
                }

                if (settings.Exec is null)
                {
                    console.WriteLine(file.Path);
                    continue;
                }

                var processArguments = arguments
                    .Select(x => x.Replace("{}", file.Path, StringComparison.Ordinal))
                    .ToList();
                if (!hasPlaceholder)
                {
                    processArguments.Add(file.Path);
                }

                int exitCode;
                try
                {
                    exitCode = await processRunner.RunAsync(settings.Exec, processArguments, ct);
                }
                catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
                {
                    console.WriteLine($"Unable to run '{settings.Exec}': {ex.Message}");
                    return 1;
                }

                if (exitCode != 0)
                {
                    console.WriteLine(
                        $"Program '{settings.Exec}' failed for '{file.Path}' with exit code {exitCode}."
                    );
                    return exitCode;
                }
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return 130;
        }
        return 0;
    }

    private static string? FindUnknownOption(CommandContext context, string[] input, int separator)
    {
        var limit = separator < 0 ? input.Length : separator;
        for (var i = 0; i < limit; i++)
        {
            var key = input[i].Split('=', 2)[0];
            if (key is "--query" or "-q" or "--exec")
            {
                if (input[i] == key)
                {
                    i++;
                }
                continue;
            }

            if (key is "--norecursive" or "--yes" or "--help" or "-h")
            {
                continue;
            }

            if (context.Remaining.Parsed.Contains(key))
            {
                return key;
            }
        }

        return null;
    }
}

using System.Reflection;

namespace TagSelecta.Shared;

public static class AppVersion
{
    public static string Get()
    {
        var informationalVersion = GetInformationalVersion();
        if (string.IsNullOrWhiteSpace(informationalVersion))
        {
            return "unknown";
        }

        var plusIndex = informationalVersion.IndexOf('+');
        return plusIndex >= 0 ? informationalVersion[..plusIndex] : informationalVersion;
    }

    private static string? GetInformationalVersion()
    {
        return GetVersionFromAssembly(Assembly.GetEntryAssembly())
            ?? GetVersionFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static string? GetVersionFromAssembly(Assembly? assembly)
    {
        return assembly
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
    }
}

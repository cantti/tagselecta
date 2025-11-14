using System.Reflection;
using Spectre.Console;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public static class TagDataActionHelper
{
    private static readonly HashSet<string> _validFields = new(StringComparer.OrdinalIgnoreCase)
    {
        TagFieldNames.AlbumArtist,
        TagFieldNames.Artist,
        TagFieldNames.Album,
        TagFieldNames.Year,
        TagFieldNames.Title,
        TagFieldNames.Track,
        TagFieldNames.TrackTotal,
        TagFieldNames.Disc,
        TagFieldNames.DiscTotal,
        TagFieldNames.Genre,
        TagFieldNames.Comment,
        TagFieldNames.Composer,
        TagFieldNames.Label,
        TagFieldNames.Pictures,
    };

    public static bool IsValid(string fieldName) =>
        !string.IsNullOrWhiteSpace(fieldName) && _validFields.Contains(fieldName);

    public static bool ValidateFieldNameList(IAnsiConsole console, IEnumerable<string> fields)
    {
        foreach (var field in fields)
        {
            if (!_validFields.Contains(field))
            {
                console.MarkupLineInterpolated($"[red]Unknown field: {field}[/]");
                return false;
            }
        }
        return true;
    }

    public static List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }
}

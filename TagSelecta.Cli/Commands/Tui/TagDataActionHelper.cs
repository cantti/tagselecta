namespace TagSelecta.Cli.Commands.Tui;

public static class TagDataActionHelper
{
    public static bool ValidateFieldNameList(IEnumerable<string> fields)
    {
        foreach (var field in fields)
        {
            if (!Fields.All.Contains(field))
            {
                return false;
            }
        }
        return true;
    }

    public static List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return list.Select(x => x.ToLower().Trim()).ToList();
    }
}

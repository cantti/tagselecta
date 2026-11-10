using TagLib;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tui;

internal static class TagDataComparison
{
    public static int GetFieldCount(TagDataActionTarget file)
    {
        return GetFields(file).Count;
    }

    public static List<Field> GetFields(TagDataActionTarget file)
    {
        var tagData = file.CurrentTagData;
        var backupTagData = file.BackupTagData;
        var fields = new List<Field>();

        AddField("Path", file.BackupPath, file.CurrentPath, file.BackupPath == file.CurrentPath);

        foreach (
            var key in backupTagData
                .Fields.Select(x => x.Key)
                .Union(tagData.Fields.Select(x => x.Key))
                .Order()
        )
        {
            var value1 = ListToStr(
                backupTagData.Fields.SingleOrDefault(x => x.Key == key)?.Text ?? []
            );
            var value2 = ListToStr(tagData.Fields.SingleOrDefault(x => x.Key == key)?.Text ?? []);
            AddField(key, value1, value2, value1 == value2);
        }

        if (backupTagData.Picture.Count > 0 || tagData.Picture.Count > 0)
        {
            var types = backupTagData
                .Picture.Select(p => p.Type)
                .Union(tagData.Picture.Select(p => p.Type))
                .OrderBy(t => t.ToString())
                .ToList();

            foreach (var type in types)
            {
                var list1 = backupTagData.Picture.Where(p => p.Type == type).ToList();
                var list2 = tagData.Picture.Where(p => p.Type == type).ToList();
                var max = Math.Max(list1.Count, list2.Count);

                for (var i = 0; i < max; i++)
                {
                    var p1 = i < list1.Count ? list1[i] : null;
                    var p2 = i < list2.Count ? list2[i] : null;
                    AddField(
                        type.ToString(),
                        PictureToStr(p1),
                        PictureToStr(p2),
                        TagDataComparer.PictureEq(p1, p2),
                        isPicture: true
                    );
                }
            }
        }

        return fields;

        void AddField(
            string key,
            string oldValue,
            string newValue,
            bool equal,
            bool isPicture = false
        )
        {
            if (!string.IsNullOrEmpty(oldValue) || !string.IsNullOrEmpty(newValue))
            {
                fields.Add(new Field(key, oldValue, newValue, equal, isPicture));
            }
        }
    }

    private static string PictureToStr(Picture? picture)
    {
        return picture is not null ? $"{picture.Data.Count / 1024} KB, {picture.MimeType}" : "";
    }

    private static string ListToStr(List<string> list)
    {
        return string.Join("\n", list);
    }

    internal record Field(string Key, string OldValue, string NewValue, bool Equal, bool IsPicture);
}

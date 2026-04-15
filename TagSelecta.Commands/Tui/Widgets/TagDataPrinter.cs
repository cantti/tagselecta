using Spectre.Console;
using Spectre.Console.Rendering;
using TagLib;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tui.Widgets;

public static class TagDataPrinter
{
    public static IRenderable PrintComparison(TagDataActionTarget f)
    {
        var tagData = f.CurrentTagData;
        var backupTagData = f.BackupTagData;

        var table = new Table();
        table.Border(TableBorder.None);
        table.AddColumn("", c => c.Width(null));
        table.AddColumn("");
        table.HideHeaders();

        AddFieldComparison(table, "Path", f.BackupPath, f.CurrentPath);

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
            AddFieldComparison(table, key, value1, value2);
        }

        if (backupTagData.Picture.Count > 0 || tagData.Picture.Count > 0)
        {
            table.AddEmptyRow();
            table.AddRow("[i]Pictures:[/]");

            // collect unique picture types
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
                    var value1 = PictureToStr(p1);
                    var value2 = PictureToStr(p2);
                    AddFieldComparison(
                        table,
                        type.ToString(),
                        value1,
                        value2,
                        TagDataComparer.PictureEq(p1, p2)
                    );
                }
            }
        }

        return table;
    }

    private static void AddFieldComparison(
        Table table,
        string key,
        string value1,
        string value2,
        bool? eq = null
    )
    {
        if (string.IsNullOrEmpty(value1) && string.IsNullOrEmpty(value2))
        {
            return;
        }

        var keyPretty = key switch
        {
            FieldName.Album => "Album",
            FieldName.AlbumArtist => "Album Artist",
            FieldName.Artist => "Artist",
            FieldName.Bpm => "BPM",
            FieldName.Comment => "Comment",
            FieldName.Composer => "Composer",
            FieldName.Conductor => "Conductor",
            FieldName.Copyright => "Copyright",
            FieldName.Date => "Date",
            FieldName.DiscNumber => "Disc Number",
            FieldName.DiscTotal => "Disc Total",
            FieldName.Genre => "Genre",
            FieldName.Isrc => "ISRC",
            FieldName.Publisher => "Publisher",
            FieldName.Title => "Title",
            FieldName.TrackNumber => "Track Number",
            FieldName.TrackTotal => "Track Total",
            _ => key,
        };

        var keyCol = new Text(keyPretty, new Style(Color.Blue));

        var elements = new List<IRenderable>();
        if (eq ?? value1 == value2)
        {
            elements.Add(new Text(value1, new Style(Color.Default)));
        }
        else
        {
            if (!string.IsNullOrEmpty(value1))
            {
                elements.Add(new Text(value1, new Style(Color.Red)));
            }

            if (!string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value1))
            {
                elements.Add(new Text(">"));
            }

            if (!string.IsNullOrEmpty(value2))
            {
                elements.Add(new Text(value2, new Style(Color.Green)));
            }
        }

        var cols = new Columns(elements) { Expand = false };
        table.AddRow(keyCol, cols);
    }

    private static string PictureToStr(Picture? pic)
    {
        return pic is not null ? $"{pic.Data.Count / 1024} KB, {pic.MimeType}".EscapeMarkup() : "";
    }

    private static string ListToStr(List<string> list)
    {
        return string.Join("\n", list);
    }
}

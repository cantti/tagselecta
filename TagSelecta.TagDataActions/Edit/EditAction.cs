using TagSelecta.Shared.TagDataActions;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataActions.Edit;

[TagDataActionName("edit", "e")]
public class EditAction : TagDataAction<EditSettings>
{
    protected override void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        EditSettings settings
    )
    {
        var tagData = current.CurrentTagData;
        var formatter = new TagDataFormatter(current.OriginalTagData, current.OriginalPath);

        Write(s => s.Album, v => tagData.Album = v);
        Write(s => s.AlbumArtist, v => tagData.AlbumArtist = v.ToMulti());
        Write(s => s.Artist, v => tagData.Artist = v.ToMulti());
        Write(s => s.Bpm, v => tagData.Bpm = v);
        Write(s => s.CatalogNumber, v => tagData.CatalogNumber = v);
        Write(s => s.Comment, v => tagData.Comment = v);
        Write(s => s.Composer, v => tagData.Composer = v.ToMulti());
        Write(s => s.Conductor, v => tagData.Conductor = v);
        Write(s => s.Copyright, v => tagData.Copyright = v);
        Write(s => s.Date, v => tagData.Date = v);
        Write(s => s.Disc, v => tagData.Disc = v);
        Write(s => s.DiscTotal, v => tagData.DiscTotal = v);
        Write(s => s.DiscogsReleaseId, v => tagData.DiscogsReleaseId = v);
        Write(s => s.Genre, v => tagData.Genre = v.ToMulti());
        Write(s => s.Isrc, v => tagData.Isrc = v);
        Write(s => s.Label, v => tagData.Label = v);
        Write(s => s.Publisher, v => tagData.Publisher = v);
        Write(s => s.Title, v => tagData.Title = v);
        Write(s => s.Track, v => tagData.Track = v);
        Write(s => s.TrackTotal, v => tagData.TrackTotal = v);

        if (settings.ClearCustom)
        {
            tagData.ClearCustomFields();
        }

        if (settings.Set is not null)
        {
            foreach (var entry in settings.Set)
            {
                var parts = entry.Split('=', 2);
                var key = parts[0].NormalizeKey();
                var value = parts.Length > 1 ? parts[1].Trim() : "";
                value = formatter.Format(value);
                WriteSet(tagData, key, value);
            }
        }

        if (settings.ClearPicture)
        {
            tagData.Picture = [];
        }

        if (settings.Picture is not null)
        {
            for (int i = 0; i < settings.Picture.Length; i++)
            {
                var path = settings.Picture[i];
                // try to find a corresponding picture type, or use first
                var typeStr =
                    settings.PictureType?.ElementAtOrDefault(i)
                    ?? settings.PictureType?.FirstOrDefault();
                var picture = new TagLib.Picture(path)
                {
                    Type =
                        !string.IsNullOrEmpty(typeStr)
                        && Enum.TryParse<TagLib.PictureType>(typeStr, true, out var type)
                            ? type
                            : TagLib.PictureType.FrontCover,
                };
                tagData.Picture.Add(picture);
            }
        }

        return;

        void Write(Func<EditSettings, string?> get, Action<string> set)
        {
            var value = get(settings);
            if (value is null)
            {
                return;
            }

            var formatted = formatter.Format(value);
            set(formatted);
        }
    }

    private static void WriteSet(TagData tagData, string key, string value)
    {
        var prop = typeof(TagData)
            .GetProperties()
            .SingleOrDefault(x =>
                x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                && (x.PropertyType == typeof(string) || x.PropertyType == typeof(List<string>))
            );
        if (prop is not null)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(tagData, value);
            }
            else
            {
                prop.SetValue(tagData, value.ToMulti());
            }
        }
        else
        {
            tagData.SetCustomField(key, value);
        }
    }
}

using TagSelecta.Cli.Commands.Common;
using TagSelecta.Shared;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Write;

public class WriteAction : TagDataAction<WriteSettings>
{
    protected override void ProcessTagData(
        TagDataOperation current,
        List<TagDataOperation> operations,
        WriteSettings settings
    )
    {
        var tagData = current.TagData;
        var formatter = new TagDataFormatter(tagData.Clone(), current.Path);

        var map = new (Func<WriteSettings, object?> get, Action<string> set)[]
        {
            (s => s.Album, v => tagData.Album = v),
            (s => s.AlbumArtist, v => tagData.AlbumArtist = v.ToMulti()),
            (s => s.Artist, v => tagData.Artist = v.ToMulti()),
            (s => s.Bpm, v => tagData.Bpm = v),
            (s => s.CatalogNumber, v => tagData.CatalogNumber = v),
            (s => s.Comment, v => tagData.Comment = v),
            (s => s.Composer, v => tagData.Composer = v.ToMulti()),
            (s => s.Conductor, v => tagData.Conductor = v),
            (s => s.Copyright, v => tagData.Copyright = v),
            (s => s.Date, v => tagData.Date = v),
            (s => s.Disc, v => tagData.Disc = v),
            (s => s.DiscTotal, v => tagData.DiscTotal = v),
            (s => s.DiscogsReleaseId, v => tagData.DiscogsReleaseId = v),
            (s => s.Genre, v => tagData.Genre = v.ToMulti()),
            (s => s.Isrc, v => tagData.Isrc = v),
            (s => s.Label, v => tagData.Label = v),
            (s => s.Publisher, v => tagData.Publisher = v),
            (s => s.Title, v => tagData.Title = v),
            (s => s.Track, v => tagData.Track = v),
            (s => s.TrackTotal, v => tagData.TrackTotal = v),
            (s => s.TrackTotal, v => tagData.TrackTotal = v),
        };

        foreach (var (get, set) in map)
        {
            var value = get(settings);
            if (value == null)
            {
                continue;
            }
            var str = (string)value;
            str = formatter.Format(str);
            set(str);
        }

        if (settings.ClearCustom)
        {
            tagData.Custom = [];
        }

        if (settings.Custom is not null)
        {
            foreach (var entry in settings.Custom)
            {
                var parts = entry.Split('=', 2);
                var key = parts[0].Trim().ToLower();
                var value = parts.Length > 1 ? parts[1].Trim() : "";

                value = formatter.Format(value);

                var customTagData = tagData.Custom.SingleOrDefault(x => x.Key == key);

                if (customTagData is not null)
                {
                    customTagData.Text = value;
                }
                else
                {
                    tagData.Custom.Add(new CustomField(key, value));
                }
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
                // try to find corresponding picture type, or use first
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
    }
}

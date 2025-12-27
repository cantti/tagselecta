using TagSelecta.Cli.Commands.Common;
using TagSelecta.Shared;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Write;

public class WriteAction : TagDataAction<WriteSettings>
{
    protected override void ProcessTagData(
        FileWithTagData current,
        List<FileWithTagData> files,
        WriteSettings settings,
        ILookup<string, string?> remainingOptions
    )
    {
        var tagData = current.TagData;
        var formatter = new TagDataFormatter(tagData, current.Path);

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
            tagData.Custom = [];
        }

        // TODO refactor code duplication etc
        foreach (var item in remainingOptions)
        {
            var key = item.Key.Trim().TrimStart('-').ToLower();
            var value = formatter.Format(item.ToJoined());
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

        return;

        void Write(Func<WriteSettings, string?> get, Action<string> set)
        {
            var value = get(settings);
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var formatted = formatter.Format(value);
            set(formatted);
        }
    }
}

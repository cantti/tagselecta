using TagLib;

namespace TagSelecta.Shared.Tagging;

public class TagData
{
    private readonly List<ExtraField> _extra = [];
    public string Album { get; set; } = "";

    public List<string> AlbumArtist { get; set; } = [];

    public List<string> Artist { get; set; } = [];

    public string Bpm { get; set; } = "";

    public string CatalogNumber { get; set; } = "";

    public string Comment { get; set; } = "";

    public List<string> Composer { get; set; } = [];

    public string Conductor { get; set; } = "";

    public string Copyright { get; set; } = "";

    public string Date { get; set; } = "";

    public string Disc { get; set; } = "";

    public string DiscTotal { get; set; } = "";

    public List<string> Genre { get; set; } = [];

    public string Isrc { get; set; } = "";

    public string Label { get; set; } = "";

    public string Publisher { get; set; } = "";

    public string Title { get; set; } = "";

    public string Track { get; set; } = "";

    public string TrackTotal { get; set; } = "";

    public List<Picture> Picture { get; set; } = [];
    public IReadOnlyList<ExtraField> Extra => _extra.OrderBy(cf => cf.Key).ToList();

    public void ClearExtraFields()
    {
        _extra.Clear();
    }

    public void SetExtraField(string key, string value)
    {
        key = key.NormalizeKey();

        var index = _extra.FindIndex(cf => cf.Key == key);

        if (string.IsNullOrWhiteSpace(value))
        {
            if (index >= 0)
            {
                _extra.RemoveAt(index);
            }
        }
        else
        {
            var replacement = new ExtraField(key, value);
            if (index < 0)
            {
                _extra.Add(replacement);
            }
            else
            {
                _extra[index] = replacement;
            }
        }
    }

    public void SetField(string key, string value)
    {
        key = key.NormalizeKey();
        value = value.Trim();
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
                prop.SetValue(this, value);
            }
            else
            {
                prop.SetValue(this, value.ToMulti());
            }
        }
        else
        {
            SetExtraField(key, value);
        }
    }

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}

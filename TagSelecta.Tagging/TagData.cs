namespace TagSelecta.Tagging;

public class TagData
{
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

    public string DiscogsReleaseId { get; set; } = "";

    public List<string> Genre { get; set; } = [];

    public string Isrc { get; set; } = "";

    public string Label { get; set; } = "";

    public string Publisher { get; set; } = "";

    public string Title { get; set; } = "";

    public string Track { get; set; } = "";

    public string TrackTotal { get; set; } = "";

    public List<TagLib.Picture> Picture { get; set; } = [];

    private readonly List<CustomField> _custom = [];
    public IReadOnlyList<CustomField> Custom => _custom.OrderBy(cf => cf.Key).ToList();

    public void ClearCustomFields() => _custom.Clear();

    public void SetCustomField(string key, string value)
    {
        key = key.NormalizeKey();

        var index = _custom.FindIndex(cf => cf.Key == key);

        if (string.IsNullOrWhiteSpace(value))
        {
            if (index >= 0)
            {
                _custom.RemoveAt(index);
            }
        }
        else
        {
            var replacement = new CustomField(key, value);
            if (index < 0)
            {
                _custom.Add(replacement);
            }
            else
            {
                _custom[index] = replacement;
            }
        }
    }

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}

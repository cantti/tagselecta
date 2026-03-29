using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class ReleaseLabel
{
    public int Id { get; set; }
    public string Catno { get; set; } = "";
    public string Name { get; set; } = "";
}

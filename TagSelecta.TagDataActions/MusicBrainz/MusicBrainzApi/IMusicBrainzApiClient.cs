using Refit;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

[Headers(
    "User-Agent: TagSelecta/1.0 +https://github.com/cantti/tagselecta",
    "Accept: application/json"
)]
public interface IMusicBrainzApiClient
{
    [Get("/release/{id}?inc=artist-credits+labels+discids+recordings")]
    Task<Release> GetRelease(string id);
}

public class RootObject { }

public class Artist_credit
{
    public string name { get; set; }
    public string joinphrase { get; set; }
    public Artist artist { get; set; }
}

public class Artist
{
    public string id { get; set; }
    public string name { get; set; }
    public string sort_name { get; set; }
    public string disambiguation { get; set; }
}

public class Release_events
{
    public string date { get; set; }
    public Area area { get; set; }
}

public class Area
{
    public string id { get; set; }
    public string name { get; set; }
    public string sort_name { get; set; }
    public string[] iso_3166_1_codes { get; set; }
    public string disambiguation { get; set; }
}

public class Label_info
{
    public string catalog_number { get; set; }
    public Label label { get; set; }
}

public class Label
{
    public string id { get; set; }
    public string name { get; set; }
    public string disambiguation { get; set; }
    public object label_code { get; set; }
}

public class Text_representation
{
    public string language { get; set; }
    public string script { get; set; }
}

public class Media
{
    public Discs[] discs { get; set; }
    public int position { get; set; }
    public string title { get; set; }
    public string format_id { get; set; }
    public string format { get; set; }
    public int track_count { get; set; }
    public int track_offset { get; set; }
    public Tracks[] tracks { get; set; }
}

public class Discs
{
    public string id { get; set; }
    public int sectors { get; set; }
    public int[] offsets { get; set; }
    public int offset_count { get; set; }
}

public class Tracks
{
    public string id { get; set; }
    public string title { get; set; }
    public int length { get; set; }
    public string number { get; set; }
    public int position { get; set; }
    public Artist_credit1[] artist_credit { get; set; }
    public Recording recording { get; set; }
}

public class Artist_credit1
{
    public string name { get; set; }
    public string joinphrase { get; set; }
    public Artist1 artist { get; set; }
}

public class Artist1
{
    public string id { get; set; }
    public string name { get; set; }
    public string sort_name { get; set; }
    public string disambiguation { get; set; }
}

public class Recording
{
    public string id { get; set; }
    public string title { get; set; }
    public string disambiguation { get; set; }
    public int length { get; set; }
    public bool video { get; set; }
    public Artist_credit2[] artist_credit { get; set; }
}

public class Artist_credit2
{
    public string name { get; set; }
    public string joinphrase { get; set; }
    public Artist2 artist { get; set; }
}

public class Artist2
{
    public string id { get; set; }
    public string name { get; set; }
    public string sort_name { get; set; }
    public string disambiguation { get; set; }
}

public class Cover_art_archive
{
    public int count { get; set; }
    public bool artwork { get; set; }
    public bool front { get; set; }
    public bool back { get; set; }
    public bool darkened { get; set; }
}

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Release
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Disambiguation { get; set; }
    public Artist_credit[] ArtistCredit { get; set; }
    public string Date { get; set; }
    public string Country { get; set; }
    public Release_events[] release_events { get; set; }
    public Label_info[] label_info { get; set; }
    public string Barcode { get; set; }
    public string PackagingId { get; set; }
    public string Packaging { get; set; }
    public string Status_id { get; set; }
    public string Status { get; set; }
    public string Quality { get; set; }
    public Text_representation Text_representation { get; set; }
    public object Asin { get; set; }
    public Media[] media { get; set; }
    public Cover_art_archive Cover_art_archive { get; set; }
}

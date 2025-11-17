namespace TagSelecta.Tagging;

public class CustomField
{
    public CustomField(string key, string text)
    {
        Key = key;
        Text = text;
    }

    private string _key = string.Empty;

    public string Key
    {
        get => _key;
        set => _key = value.ToLowerInvariant();
    }

    public string Text { get; set; }
}

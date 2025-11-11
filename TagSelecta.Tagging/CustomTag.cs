namespace TagSelecta.Tagging;

public class CustomTag
{
    public CustomTag(string key, string value)
    {
        Key = key;
        Value = value;
    }

    private string _key = string.Empty;

    public string Key
    {
        get => _key;
        set => _key = value.ToLowerInvariant();
    }

    public string Value { get; set; }
}

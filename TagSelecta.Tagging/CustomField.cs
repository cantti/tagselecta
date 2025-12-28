using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class CustomField
{
    public CustomField(string key, string text)
    {
        Key = !string.IsNullOrEmpty(key)
            ? key.NormalizeKey()
            : throw new ArgumentException("Key cannot be null or empty", nameof(key));
        Text = text; // Goes through the property setter for validation
    }

    public string Key { get; }

    private string _text = string.Empty;
    public string Text
    {
        get => _text;
        set =>
            _text = value ?? throw new ArgumentNullException(nameof(Text), "Text cannot be null");
    }
}
